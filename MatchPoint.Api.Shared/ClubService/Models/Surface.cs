using MatchPoint.Api.Shared.ClubService.Enums;

namespace MatchPoint.Api.Shared.ClubService.Models
{
    public class Surface
    {
        public SurfaceType Type { get; set; }
        public SurfaceMaterial Material { get; set; }
        public SurfaceTexture Texture { get; set; }
    }
}
