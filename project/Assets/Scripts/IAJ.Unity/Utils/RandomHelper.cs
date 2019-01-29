using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Utils
{
    public static class RandomHelper
    {
        public static float RandomBinomial(float max)
        {
            return Random.Range(0, max) - Random.Range(0, max);
        }

        public static float RandomBinomial()
        {
            return Random.Range(0, 1.0f) - Random.Range(0, 1.0f);
        }

		public static int RollD6()
		{
			return Random.Range(1, 7);
		}

		public static int RollD10()
		{
			return Random.Range(1, 11);
		}

		public static int RollD12()
		{
			return Random.Range(1, 13);
		}

		public static int RollD20() 
		{
			return Random.Range(1, 21);
		}
    }
}
