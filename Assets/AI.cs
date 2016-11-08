using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {
	public enum Entity{AI, Other};
	enum State{Covered, Uncovered, badPosition}
	public int steps;
	public GameObject self;
	public class DecisionNode{
		
		public Entity entity;

		int totalWeightHere;

		float AI_HP;
		float O_HP;
		float defenseMult;
		float offenseMult;

	}
	public char[] think(int steps){
		char[] actions = {'a','b'};
		DecisionNode thisDecisionNode = new DecisionNode();
		State thisState;
		//for debugging
		thisState = State.Covered;
		//end of debugging values
		thisDecisionNode.entity = Entity.AI;
		int attackChoice;
		int defenseChoice;
		max(thisDecisionNode, thisState, steps - 1);

		return actions;
	}

	private int min(DecisionNode currDecisionNode, State state , int steps){
		return 0;
	}
	private int max(DecisionNode currDecisionNode, State state, int steps){
		return 0;
	}

	State getState(){
		return State.Uncovered;
	}

}
