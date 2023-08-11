

public static class MultiCraft 
{

    public static System.Random random = new System.Random(); // can be marked as static in the gamemanager ! to acces from everywhere 

    public static bool IsMultiCraft(float multicraftChanceModifier_IN)
    {
        float rnd = (float)random.NextDouble();
        rnd *= 100;
        DebuggerForNonMono.Logger(rnd + " : multicraft Roll");

        if(rnd <= multicraftChanceModifier_IN)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
