using UnityEditor;
using UniGLTF.M17N;

namespace UniGLTF.EditorSettingsValidator
{
    public sealed class UnityColorSpaceSettingsValidator : IUnitySettingsValidator
    {
        public bool IsValid =>
#if UNIGLTF_USE_GAMMA_COLORSPACE
                    true
#else
                    PlayerSettings.colorSpace == UnityEngine.ColorSpace.Gamma
#endif
                    ;

        public string HeaderDescription => Messages.ColorSpace.Msg();
        public string CurrentValueDescription => PlayerSettings.colorSpace == UnityEngine.ColorSpace.Gamma
            ? Messages.ColorSpaceLinear.Msg() : Messages.ColorSpaceGamma.Msg();
        public string RecommendedValueDescription => Messages.ColorSpaceLinear.Msg();

        public void Validate()
        {
            PlayerSettings.colorSpace = UnityEngine.ColorSpace.Gamma;
        }
    }
}
