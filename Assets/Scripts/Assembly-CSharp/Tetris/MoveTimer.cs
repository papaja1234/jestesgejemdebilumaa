using UnityEngine;

public class MoveTimer
{
    private readonly KeyCode key;
    private int heldTicks;

    public MoveTimer(KeyCode key)
    {
        this.key = key;
        this.heldTicks = 0;
    }

    public void Tick()
    {
        if (Input.GetKey(this.key))
            this.heldTicks++;
        else
            this.heldTicks = 0;
    }

    public bool ShouldMove(bool cm)
    {
        return Input.GetKeyDown(this.key) || (cm && this.heldTicks > 20);
    }
}