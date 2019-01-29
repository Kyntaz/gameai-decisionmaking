namespace Assets.Scripts.IAJ.Unity.Utils
{
    public class Pair<T1,T2>
    {
        public T1 Left { get; set; }
        public T2 Right { get; set; }

        public Pair(T1 left, T2 right)
        {
            this.Left = left;
            this.Right = right;
        }

        public override bool Equals(object obj)
        {
            Pair<T1, T2> objPair;

            if(obj is Pair<T1,T2>)
            {
                objPair = (Pair<T1, T2>)obj;
                return this.Left.Equals(objPair.Left) && this.Right.Equals(objPair.Right); 
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (this.Left.ToString() + this.Right).GetHashCode();
        }
    }
}
