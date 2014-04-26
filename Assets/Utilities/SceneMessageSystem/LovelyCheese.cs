using UnityEngine;

public class LovelyCheese
{
	public enum CheeseType{Brie, Cheddar, Feta};
	
	public CheeseType cheeseType;
	
	public LovelyCheese()
	{
		cheeseType = (CheeseType) Random.Range( 0, 3 );
	}
}