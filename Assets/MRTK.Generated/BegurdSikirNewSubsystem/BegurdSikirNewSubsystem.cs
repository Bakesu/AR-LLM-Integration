// TODO: [Optional] Add copyright and license statement(s).

using MixedReality.Toolkit;
using MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Scripting;

namespace BegurdSikir.MRTK3.Subsystems
{
    [Preserve]
    [MRTKSubsystem(
        Name = "begurdsikir.mrtk3.subsystems",
        DisplayName = "BegurdSikir NewSubsystem",
        Author = "BegurdSikir",
        ProviderType = typeof(BegurdSikirNewSubsystemProvider),
        SubsystemTypeOverride = typeof(BegurdSikirNewSubsystem),
        ConfigType = typeof(BaseSubsystemConfig))]
    public class BegurdSikirNewSubsystem : NewSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<BegurdSikirNewSubsystem, NewSubsystemCinfo>();

            if (!BegurdSikirNewSubsystem.Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        [Preserve]
        class BegurdSikirNewSubsystemProvider : Provider
        {

            #region INewSubsystem implementation

            // TODO: Add the provider implementation.

            #endregion NewSubsystem implementation
        }
    }
}
