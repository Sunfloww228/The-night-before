using UnityEngine;

public abstract class DistractionBase : MonoBehaviour
{
    protected GameManager Game; 

    public bool IsActive { get; private set; }

    public void Initialize(GameManager game)
    {
        Game = game;
    }

    public virtual void Activate() 
    {
        IsActive = true;
        gameObject.SetActive(true);
    }

    public virtual void Tick(float dk) { }
    public virtual void Resolve()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }
}
