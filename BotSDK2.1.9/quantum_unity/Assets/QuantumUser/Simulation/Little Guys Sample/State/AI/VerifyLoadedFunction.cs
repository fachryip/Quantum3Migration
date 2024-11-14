using System;

namespace Quantum
{
    [Serializable]
    public class VerifyLoadedFunction : AIFunction<bool>
    {
        public override unsafe bool Execute(Frame f, EntityRef e, ref AIContext aiContext)
        {
            var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);

            return guyComponent->BotData.Loaded == true;
        }
    }
}