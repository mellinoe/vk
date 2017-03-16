using System.Numerics;

namespace Vk.Samples
{
    public struct VertexPositionNormalTexture
    {
        public const byte SizeInBytes = 32;
        public const byte NormalOffset = 12;
        public const byte TextureCoordinatesOffset = 24;
        public const byte ElementCount = 3;

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinates;

        public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 texCoords)
        {
            Position = position;
            Normal = normal;
            TextureCoordinates = texCoords;
        }
    }
}