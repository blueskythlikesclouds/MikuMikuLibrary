using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Hashes;
using MikuMikuLibrary.Skeletons;

namespace MikuMikuLibrary.Motions
{
    public class MotionBinding
    {
        public Motion Parent { get; }

        public List<BoneBinding> BoneBindings { get; }
        public BoneBinding GlobalTransformation { get; }

        public void Unbind( Skeleton skeleton, MotionDatabase motionDatabase = null )
        {
            Parent.KeySets.Clear();
            Parent.BoneInfos.Clear();

            foreach ( var boneBinding in skeleton.MotionBoneNames
                .Where( x => motionDatabase == null || motionDatabase.BoneNames.Contains( x, StringComparer.OrdinalIgnoreCase ) )
                .Select( x => BoneBindings.Find( y => y.Name.Equals( x, StringComparison.OrdinalIgnoreCase ) ) ?? new BoneBinding { Name = x } )
                .OrderBy( x => sReferenceIndices.TryGetValue( x.Name, out int index ) ? index : int.MaxValue ) )
            {
                var bone = skeleton.GetBone( boneBinding.Name );

                if ( bone != null )
                {
                    if ( bone.Type != BoneType.Rotation )
                        UnbindPosition( boneBinding );

                    if ( bone.Type != BoneType.Position )
                        UnbindRotation( boneBinding );
                }

                AddBone( boneBinding.Name );
            }

            AddBone( "gblctr" );
            UnbindPosition( GlobalTransformation );

            AddBone( "kg_ya_ex" );
            UnbindRotation( GlobalTransformation );

            Parent.KeySets.Add( new KeySet() );

            void UnbindPosition( BoneBinding boneBinding )
            {
                Parent.KeySets.Add( boneBinding.Position?.X ?? new KeySet() );
                Parent.KeySets.Add( boneBinding.Position?.Y ?? new KeySet() );
                Parent.KeySets.Add( boneBinding.Position?.Z ?? new KeySet() );
            }

            void UnbindRotation( BoneBinding boneBinding )
            {
                Parent.KeySets.Add( boneBinding.Rotation?.X ?? new KeySet() );
                Parent.KeySets.Add( boneBinding.Rotation?.Y ?? new KeySet() );
                Parent.KeySets.Add( boneBinding.Rotation?.Z ?? new KeySet() );
            }

            void AddBone( string name )
            {
                Parent.BoneInfos.Add( new BoneInfo
                {
                    Name = name,
                    Id = ( uint? ) motionDatabase?.BoneNames?.FindIndex( 
                        x => x.Equals( name, StringComparison.OrdinalIgnoreCase ) ) ?? MurmurHash.Calculate( name )
                } );
            }
        }

        public void Merge( MotionBinding other )
        {
            foreach ( var boneBinding in other.BoneBindings )
            {
                var baseBoneBinding = BoneBindings.FirstOrDefault( x =>
                    x.Name.Equals( boneBinding.Name, StringComparison.OrdinalIgnoreCase ) );

                if ( baseBoneBinding == null )
                    BoneBindings.Add( boneBinding );

                else
                    baseBoneBinding.Merge( boneBinding );
            }

            GlobalTransformation.Merge( other.GlobalTransformation );

            Parent.FrameCount = Math.Max( Parent.FrameCount, other.Parent.FrameCount );
        }

        public void Sort()
        {
            foreach ( var boneBinding in BoneBindings )
                boneBinding.Sort();
        }

        public MotionBinding( Motion parent )
        {
            Parent = parent;
            BoneBindings = new List<BoneBinding>();
            GlobalTransformation = new BoneBinding();
        }

        private static readonly Dictionary<string, int> sReferenceIndices =
            new Dictionary<string, int>( StringComparer.OrdinalIgnoreCase )
            {
                { "n_hara_cp", 0 },
                { "kg_hara_y", 1 },
                { "kl_hara_xz", 2 },
                { "kl_hara_etc", 3 },
                { "n_hara", 4 },
                { "e_mune_cp", 5 },
                { "cl_mune", 6 },
                { "n_mune_b", 7 },
                { "kl_mune_b_wj", 8 },
                { "kl_kubi", 9 },
                { "n_kao", 10 },
                { "e_kao_cp", 11 },
                { "cl_kao", 12 },
                { "face_root", 13 },
                { "kl_eye_l_wj", 14 },
                { "kl_eye_r_wj", 15 },
                { "kl_mabu_d_l_wj", 16 },
                { "kl_mabu_d_r_wj", 17 },
                { "kl_mabu_l_wj", 18 },
                { "kl_mabu_r_wj", 19 },
                { "kl_mabu_u_l_wj", 20 },
                { "kl_mabu_u_r_wj", 21 },
                { "n_ago", 22 },
                { "tl_ago", 23 },
                { "kl_ago_wj", 24 },
                { "n_tooth_under", 25 },
                { "n_ago_b", 26 },
                { "tl_ago_wj", 27 },
                { "tl_tooth_under_wj", 28 },
                { "tl_ha_wj", 29 },
                { "n_hoho_b_l", 30 },
                { "n_eye_l", 31 },
                { "kl_eye_l", 32 },
                { "n_eye_l_wj_ex", 33 },
                { "kl_highlight_l_wj", 34 },
                { "tl_hoho_b_l_wj", 35 },
                { "n_hoho_b_r", 36 },
                { "tl_hoho_b_r_wj", 37 },
                { "n_hoho_c_l", 38 },
                { "tl_hoho_c_l_wj", 39 },
                { "n_hoho_c_r", 40 },
                { "tl_hoho_c_r_wj", 41 },
                { "n_hoho_l", 42 },
                { "n_eye_r", 43 },
                { "kl_eye_r", 44 },
                { "n_eye_r_wj_ex", 45 },
                { "kl_highlight_r_wj", 46 },
                { "n_eyelid_l_a", 47 },
                { "tl_eyelid_l_a_wj", 48 },
                { "tl_hoho_l_wj", 49 },
                { "n_hoho_r", 50 },
                { "tl_hoho_r_wj", 51 },
                { "n_eyelid_l_b", 52 },
                { "tl_eyelid_l_b_wj", 53 },
                { "n_eyelid_r_a", 54 },
                { "tl_eyelid_r_a_wj", 55 },
                { "n_eyelid_r_b", 56 },
                { "tl_eyelid_r_b_wj", 57 },
                { "n_kuti_d", 58 },
                { "tl_kuti_d_wj", 59 },
                { "n_kuti_d_l", 60 },
                { "tl_kuti_d_l_wj", 61 },
                { "n_kuti_d_r", 62 },
                { "tl_kuti_d_r_wj", 63 },
                { "n_kuti_ds_l", 64 },
                { "tl_kuti_ds_l_wj", 65 },
                { "n_kuti_ds_r", 66 },
                { "tl_kuti_ds_r_wj", 67 },
                { "n_kuti_l", 68 },
                { "tl_kuti_l_wj", 69 },
                { "n_kuti_m_l", 70 },
                { "tl_kuti_m_l_wj", 71 },
                { "n_kuti_m_r", 72 },
                { "tl_kuti_m_r_wj", 73 },
                { "n_kuti_r", 74 },
                { "tl_kuti_r_wj", 75 },
                { "n_kuti_u", 76 },
                { "tl_kuti_u_wj", 77 },
                { "n_kuti_u_l", 78 },
                { "tl_kuti_u_l_wj", 79 },
                { "n_kuti_u_r", 80 },
                { "tl_kuti_u_r_wj", 81 },
                { "n_mabu_l_d_a", 82 },
                { "tl_mabu_l_d_a_wj", 83 },
                { "n_mabu_l_d_b", 84 },
                { "tl_mabu_l_d_b_wj", 85 },
                { "n_mabu_l_d_c", 86 },
                { "tl_mabu_l_d_c_wj", 87 },
                { "n_mabu_l_u_a", 88 },
                { "tl_mabu_l_u_a_wj", 89 },
                { "n_mabu_l_u_b", 90 },
                { "tl_mabu_l_u_b_wj", 91 },
                { "n_mabu_l_d", 92 },
                { "tl_mabu_l_d_wj", 93 },
                { "n_mabu_l_d_l", 94 },
                { "tl_mabu_l_d_l_wj", 95 },
                { "n_mabu_l_d_r", 96 },
                { "tl_mabu_l_d_r_wj", 97 },
                { "n_mabu_l_u", 98 },
                { "tl_mabu_l_u_wj", 99 },
                { "n_mabu_l_u_l", 100 },
                { "tl_mabu_l_u_l_wj", 101 },
                { "n_eyelashes_l", 102 },
                { "tl_eyelashes_l_wj", 103 },
                { "n_mabu_l_u_c", 104 },
                { "n_mabu_l_u_r", 105 },
                { "tl_mabu_l_u_c_wj", 106 },
                { "n_mabu_r_d_a", 107 },
                { "tl_mabu_r_d_a_wj", 108 },
                { "n_mabu_r_d_b", 109 },
                { "tl_mabu_r_d_b_wj", 110 },
                { "n_mabu_r_d_c", 111 },
                { "tl_mabu_r_d_c_wj", 112 },
                { "n_mabu_r_u_a", 113 },
                { "tl_mabu_r_u_a_wj", 114 },
                { "n_mabu_r_u_b", 115 },
                { "tl_mabu_r_u_b_wj", 116 },
                { "tl_mabu_l_u_r_wj", 117 },
                { "n_mabu_r_d", 118 },
                { "tl_mabu_r_d_wj", 119 },
                { "n_mabu_r_d_l", 120 },
                { "tl_mabu_r_d_l_wj", 121 },
                { "n_mabu_r_d_r", 122 },
                { "tl_mabu_r_d_r_wj", 123 },
                { "n_mabu_r_u", 124 },
                { "tl_mabu_r_u_wj", 125 },
                { "n_mabu_r_u_l", 126 },
                { "tl_mabu_r_u_l_wj", 127 },
                { "n_mabu_r_u_r", 128 },
                { "tl_mabu_r_u_r_wj", 129 },
                { "n_eyelashes_r", 130 },
                { "n_mayu_b_l", 131 },
                { "tl_eyelashes_r_wj", 132 },
                { "tl_mayu_b_l_wj", 133 },
                { "n_mabu_r_u_c", 134 },
                { "n_mayu_b_r", 135 },
                { "tl_mayu_b_r_wj", 136 },
                { "n_mayu_c_l", 137 },
                { "tl_mayu_c_l_wj", 138 },
                { "n_mayu_c_r", 139 },
                { "tl_mabu_r_u_c_wj", 140 },
                { "tl_mayu_c_r_wj", 141 },
                { "n_mayu_l", 142 },
                { "tl_mayu_l_wj", 143 },
                { "n_mayu_l_b", 144 },
                { "tl_mayu_l_b_wj", 145 },
                { "n_mayu_l_c", 146 },
                { "tl_mayu_l_c_wj", 147 },
                { "n_mayu_r", 148 },
                { "tl_mayu_r_wj", 149 },
                { "n_mayu_r_b", 150 },
                { "tl_mayu_r_b_wj", 151 },
                { "n_mayu_r_c", 152 },
                { "tl_mayu_r_c_wj", 153 },
                { "n_tooth_upper", 154 },
                { "tl_tooth_upper_wj", 155 },
                { "n_kubi_wj_ex", 156 },
                { "n_waki_l", 157 },
                { "kl_waki_l_wj", 158 },
                { "e_ude_l_cp", 159 },
                { "c_kata_l", 160 },
                { "kl_te_l_wj", 161 },
                { "n_hito_l_ex", 162 },
                { "nl_hito_l_wj", 163 },
                { "nl_hito_b_l_wj", 164 },
                { "nl_hito_c_l_wj", 165 },
                { "n_ko_l_ex", 166 },
                { "nl_ko_l_wj", 167 },
                { "nl_ko_b_l_wj", 168 },
                { "nl_ko_c_l_wj", 169 },
                { "n_kusu_l_ex", 170 },
                { "nl_kusu_l_wj", 171 },
                { "nl_kusu_b_l_wj", 172 },
                { "nl_kusu_c_l_wj", 173 },
                { "n_naka_l_ex", 174 },
                { "nl_naka_l_wj", 175 },
                { "nl_naka_b_l_wj", 176 },
                { "nl_naka_c_l_wj", 177 },
                { "n_oya_l_ex", 178 },
                { "nl_oya_l_wj", 179 },
                { "nl_oya_b_l_wj", 180 },
                { "nl_oya_c_l_wj", 181 },
                { "n_ste_l_wj_ex", 182 },
                { "n_sude_l_wj_ex", 183 },
                { "n_sude_b_l_wj_ex", 184 },
                { "n_hiji_l_wj_ex", 185 },
                { "n_up_kata_l_ex", 186 },
                { "n_skata_l_wj_cd_ex", 187 },
                { "n_skata_b_l_wj_cd_cu_ex", 188 },
                { "n_skata_c_l_wj_cd_cu_ex", 189 },
                { "n_waki_r", 190 },
                { "kl_waki_r_wj", 191 },
                { "e_ude_r_cp", 192 },
                { "c_kata_r", 193 },
                { "kl_te_r_wj", 194 },
                { "n_hito_r_ex", 195 },
                { "nl_hito_r_wj", 196 },
                { "nl_hito_b_r_wj", 197 },
                { "nl_hito_c_r_wj", 198 },
                { "n_ko_r_ex", 199 },
                { "nl_ko_r_wj", 200 },
                { "nl_ko_b_r_wj", 201 },
                { "nl_ko_c_r_wj", 202 },
                { "n_kusu_r_ex", 203 },
                { "nl_kusu_r_wj", 204 },
                { "nl_kusu_b_r_wj", 205 },
                { "nl_kusu_c_r_wj", 206 },
                { "n_naka_r_ex", 207 },
                { "nl_naka_r_wj", 208 },
                { "nl_naka_b_r_wj", 209 },
                { "nl_naka_c_r_wj", 210 },
                { "n_oya_r_ex", 211 },
                { "nl_oya_r_wj", 212 },
                { "nl_oya_b_r_wj", 213 },
                { "nl_oya_c_r_wj", 214 },
                { "n_ste_r_wj_ex", 215 },
                { "n_sude_r_wj_ex", 216 },
                { "n_sude_b_r_wj_ex", 217 },
                { "n_hiji_r_wj_ex", 218 },
                { "n_up_kata_r_ex", 219 },
                { "n_skata_r_wj_cd_ex", 220 },
                { "n_skata_b_r_wj_cd_cu_ex", 221 },
                { "n_skata_c_r_wj_cd_cu_ex", 222 },
                { "tl_up_kata_l", 223 },
                { "tl_up_kata_r", 224 },
                { "kl_kosi_y", 225 },
                { "kl_kosi_xz", 226 },
                { "kl_kosi_etc_wj", 227 },
                { "e_sune_l_cp", 228 },
                { "cl_momo_l", 229 },
                { "kl_asi_l_wj_co", 230 },
                { "kl_toe_l_wj", 231 },
                { "n_hiza_l_wj_ex", 232 },
                { "e_sune_r_cp", 233 },
                { "cl_momo_r", 234 },
                { "kl_asi_r_wj_co", 235 },
                { "kl_toe_r_wj", 236 },
                { "n_hiza_r_wj_ex", 237 },
                { "n_momo_a_l_wj_cd_ex", 238 },
                { "n_momo_l_cd_ex", 239 },
                { "n_momo_b_l_wj_ex", 240 },
                { "n_momo_c_l_wj_ex", 241 },
                { "n_momo_a_r_wj_cd_ex", 242 },
                { "n_momo_r_cd_ex", 243 },
                { "n_momo_b_r_wj_ex", 244 },
                { "n_momo_c_r_wj_ex", 245 },
                { "n_hara_cd_ex", 246 },
                { "n_hara_b_wj_ex", 247 },
                { "n_hara_c_wj_ex", 248 }
            };
    }
}