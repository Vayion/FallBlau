
public abstract class LandCombatStats
{
    
    //TODO: move
    protected double org, maxOrg;
    protected double hp, maxHp;

    protected double recoveryRate;
    // endTODO
    
    //TODO: modifiers in battalions
    
    
    protected double softAttack, hardAttack;
    protected double defense, breakthrough;

    protected double strength;

    protected double hardness;
    protected double armor;

    protected double speed;
    

    public void changeOrg(double diff)
    {
        org += diff;
    }
}
