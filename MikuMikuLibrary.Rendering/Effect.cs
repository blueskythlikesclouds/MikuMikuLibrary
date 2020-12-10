using System;
using System.Linq;
using System.Numerics;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.Lights;
using MikuMikuLibrary.Rendering.Extensions;
using MikuMikuLibrary.Rendering.IBLs;

namespace MikuMikuLibrary.Rendering
{
    public class Effect : IDisposable
    {
        public IBL IBL { get; set; }

        public FaceParameter FaceParameter { get; set; }
        public FogParameter FogParameter { get; set; }
        public GlowParameter GlowParameter { get; set; }
        public LightParameter LightParameter { get; set; }

        public void Bind( Renderer renderer )
        {
            IBL.Bind( renderer.State );

            var charaLight = LightParameter.Groups[ 0 ].Lights.First( x => x.Id == LightId.Character );
            var stageLight = LightParameter.Groups[ 0 ].Lights.First( x => x.Id == LightId.Stage );

            var iblSpace = Matrix4x4.CreateLookAt( Vector3.Zero, IBL.SourceIBL.Lights[ 0 ].Direction, Vector3.UnitY );
            var lightSpace = Matrix4x4.CreateLookAt( Vector3.Zero, charaLight.Position.To3D(), Vector3.UnitY );

            Matrix4x4.Invert( iblSpace, out iblSpace );

            renderer.SceneUniformBuffer.SetData( renderer.State, new SceneData
            {
                IBL = new IBLData
                {
                    FrontLight = new Vector4( IBL.SourceIBL.Lights[ 0 ].Color, 1.0f ),
                    BackLight = new Vector4( IBL.SourceIBL.Lights[ 2 ].Color, 1.0f ),
                    IrradianceR = IBL.SourceIBL.DiffuseCoefficients[ 0 ].R,
                    IrradianceG = IBL.SourceIBL.DiffuseCoefficients[ 0 ].G,
                    IrradianceB = IBL.SourceIBL.DiffuseCoefficients[ 0 ].B,
                    IBLSpace = lightSpace * iblSpace // how TF do I calculate this properly
                },
                CharaLight = charaLight.ToLightData(),
                StageLight = stageLight.ToLightData()
            } );
        }

        public void Dispose()
        {
            IBL.Dispose();
        }
    }
}