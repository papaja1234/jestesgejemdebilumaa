/// <summary>
/// fast 2d array
/// </summary>
public class Arr2D<T>
{
    public readonly T[] Raw;
    public readonly int Width, Height;

    private readonly T outOfBoundsVal;
    
    public Arr2D(int w, int h, T outOfBoundsValue)
    {
        this.Width = w;
        this.Height = h;
        this.Raw = new T[w * h];
        this.outOfBoundsVal = outOfBoundsValue;
    }

    public T this[int x, int y]
    {
        get
        {
            if (OutOfBounds(x, y)) return this.outOfBoundsVal;
            return this.Raw[x + y * this.Width];
        }
        set
        {
            if (OutOfBounds(x, y)) return;
            this.Raw[x + y * this.Width] = value;
        }
    }

    public bool OutOfBounds(int x, int y)
    {
        return x < 0 || x >= this.Width ||
               y < 0 || y >= this.Height;
    }
}