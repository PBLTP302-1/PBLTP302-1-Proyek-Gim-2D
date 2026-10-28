using UnityEngine;

public class WanderState : IState
{
    private WildMonster monster;
    private float timer;
    private Vector2 targetPoint;

    public WanderState(WildMonster monster) { this.monster = monster; }

    public void Enter() 
    { 
        monster.ShowAlert(false); 
        PickNewPoint(); 
    }
    
    public void Tick(float dt)
    {
        timer -= dt;
        monster.MoveTowards(targetPoint, 1.125f, dt); // Kecepatan jalan santai
        
        if (timer <= 0f || Vector2.Distance(monster.transform.position, targetPoint) < 0.1f)
            PickNewPoint();
    }
    
    public void Exit() { }

    private void PickNewPoint()
    {
        timer = Random.Range(1f, 3f);
        targetPoint = monster.HomePosition + Random.insideUnitCircle * 1.5f;
    }
}

public class ChaseState : IState
{
    private WildMonster monster;
    public ChaseState(WildMonster monster) { this.monster = monster; }

    public void Enter() => monster.ShowAlert(true); // Munculkan tanda !
    public void Tick(float dt) => monster.MoveTowards(monster.PlayerTransform.position, 2.5f, dt); // Kejar cepat
    public void Exit() => monster.ShowAlert(false);
}

public class ReturnState : IState
{
    private WildMonster monster;
    public ReturnState(WildMonster monster) { this.monster = monster; }

    public void Enter() => monster.ShowAlert(false);
    public void Tick(float dt) => monster.MoveTowards(monster.HomePosition, 2.5f, dt);
    public void Exit() { }
}