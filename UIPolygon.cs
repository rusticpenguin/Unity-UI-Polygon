/// Credit CiaccoDavide
/// Sourced from - http://ciaccodavi.de/unity/uipolygon


namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UI Polygon")]
    public class UIPolygon : MaskableGraphic
    {
        [SerializeField]
        Texture m_Texture;
        public bool fill = true;
        public float thickness = 5;
        [Range(3, 360)]
        public int sides = 3;
        [Range(0, 360)]
        public float rotation = 0;
        [Range(0, 1)]
        public float[] VerticesDistances = new float[3];
        [Tooltip("Keeps the image height/width locked to the lower of the two")]
        public bool lockAspectRatio = false;
        private float size = 0;

        public override Texture mainTexture
        {
            get
            {
                return m_Texture == null ? s_WhiteTexture : m_Texture;
            }
        }
        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value) return;
                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }
        public void DrawPolygon(int _sides)
        {
            sides = _sides;
            VerticesDistances = new float[_sides + 1];
            for (int i = 0; i < _sides; i++) VerticesDistances[i] = 1; ;
            rotation = 0;
        }
        public void DrawPolygon(int _sides, float[] _VerticesDistances)
        {
            sides = _sides;
            VerticesDistances = _VerticesDistances;
            rotation = 0;
        }
        public void DrawPolygon(int _sides, float[] _VerticesDistances, float _rotation)
        {
            sides = _sides;
            VerticesDistances = _VerticesDistances;
            rotation = _rotation;
        }
        void Update()
        {
            size = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height);
            thickness = (float)Mathf.Clamp(thickness, 0, size / 2);
        }
        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;
            Vector2 uv0 = new Vector2(0, 0);
            Vector2 uv1 = new Vector2(0, 1);
            Vector2 uv2 = new Vector2(1, 1);
            Vector2 uv3 = new Vector2(1, 0);
            Vector2 pos0;
            Vector2 pos1;
            Vector2 pos2;
            Vector2 pos3;
            float degrees = 360f / sides;
            int vertices = sides + 1;
            if (VerticesDistances.Length != vertices)
            {
                VerticesDistances = new float[vertices];
                for (int i = 0; i < vertices - 1; i++) VerticesDistances[i] = 1;
            }
            // last vertex is also the first!
            VerticesDistances[vertices - 1] = VerticesDistances[0];

            Rect rect = rectTransform.rect;
            Vector2 center = rect.center;
            float radiusX = rect.width * 0.5f;
            float radiusY = rect.height * 0.5f;
            if (lockAspectRatio)
            {
                float r = Mathf.Min(radiusX, radiusY);
                radiusX = r;
                radiusY = r;
            }
            float thick = Mathf.Clamp(thickness, 0, Mathf.Min(radiusX, radiusY));

            for (int i = 0; i < vertices; i++)
            {
                float d = VerticesDistances[i];
                float outerX = radiusX * d;
                float outerY = radiusY * d;
                float innerX = Mathf.Max(0, outerX - thick);
                float innerY = Mathf.Max(0, outerY - thick);
                float rad = Mathf.Deg2Rad * (i * degrees + rotation);

                float c = -Mathf.Cos(rad);
                float s = -Mathf.Sin(rad);
                uv0 = new Vector2(0, 1);
                uv1 = new Vector2(1, 1);
                uv2 = new Vector2(1, 0);
                uv3 = new Vector2(0, 0);
                pos0 = prevX;
                pos1 = center + new Vector2(outerX * c, outerY * s);
                if (fill)
                {
                    pos2 = center;
                    pos3 = center;
                }
                else
                {
                    pos2 = center + new Vector2(innerX * c, innerY * s);
                    pos3 = prevY;
                }
                prevX = pos1;
                prevY = pos2;
                vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
            }
        }
    }
}
