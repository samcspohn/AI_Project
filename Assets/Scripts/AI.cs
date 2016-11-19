using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {




	public enum State{attacking, defending}
	public int Steps;
	public GameObject self;
	public Enemy selfE;

	public Ghost SelfG;
	public Ghost otherG;
	public GameObject other;

	public Enemy otherE;

	public class DecisionNode{

		public Vector3 previousLocation1;
		public Vector3 previousLocation2;
		public State state;
		public bool otherIsVisible;

		public int totalWeightHere;

		public float AI_HP;
		public float O_HP;
		//public float defenseMult;
		//public float offenseMult;

	}
	void Start () {
		selfE = transform.GetComponentInParent<Enemy>();
		Debug.Log((selfE != null ? "got enemy component" : "no enemy component found"));
	}

	public char think(int steps){
		if(other == null){
			return ' ';
		}
		SelfG.transform.position = transform.position;
		otherG.transform.position = other.transform.position;
		SelfG.currentNavNodeID = selfE.currentNavNodeID;
		SelfG.previousNavNodeID = selfE.previousNavNodeID;
		otherG.currentNavNodeID = otherE.currentNavNodeID;
		otherG.previousNavNodeID = otherE.previousNavNodeID;

		DecisionNode attackDecision = new DecisionNode(){
			previousLocation1 = SelfG.transform.position,
			previousLocation2 = other.transform.position,
			state = State.attacking,
			otherIsVisible = isOtherVisible(),
			AI_HP = SelfG.hp,
			O_HP = otherG.hp
		};
		DecisionNode defendDecision = new DecisionNode(){
			previousLocation1 = SelfG.transform.position,
			previousLocation2 = other.transform.position,
			state = State.defending,
			otherIsVisible = isOtherVisible(),
			AI_HP = SelfG.hp,
			O_HP = otherG.hp
		};

		float beta = max(attackDecision, Steps - 1);
		float alpha = max(defendDecision, Steps - 1);
		if(Random.value > 0.7f){
			Debug.Log("O");
			return 'o';
		}else{
			if(alpha < beta){
					Debug.Log("O");
					return 'o';
			}else{
				Debug.Log("P");
				return 'p'; 
			}
				
			//selfE.takeCover();
		}

		// return actions;
	}
	public bool EisVisible(){
		RaycastHit hit;
		if(Physics.Raycast(self.transform.position,other.transform.position - self.transform.position, out hit)){
			if(hit.collider.gameObject == other){
				return true;
			}else{
				return false;
			}
		}
		return false;
	}

	private float min(DecisionNode currDecisionNode, int steps){
		
		if(currDecisionNode.state == State.attacking && currDecisionNode.otherIsVisible){
			otherG.shoot();
			SelfG.patrol();
			currDecisionNode.AI_HP -= Random.value * 5 + 10;
		}else if(currDecisionNode.state == State.attacking && !currDecisionNode.otherIsVisible){
			otherG.patrol();
		}else{
			otherG.takeCover();
		}
		DecisionNode attackDecision = new DecisionNode(){
			previousLocation1 = SelfG.transform.position,
			previousLocation2 = otherG.transform.position,
			state = State.attacking,
			otherIsVisible = isOtherVisible(),
			AI_HP = currDecisionNode.AI_HP,
			O_HP = currDecisionNode.O_HP
		};
		DecisionNode defendDecision = new DecisionNode(){
			previousLocation1 = SelfG.transform.position,
			previousLocation2 = other.transform.position,
			state = State.defending,
			otherIsVisible = isOtherVisible(),
			AI_HP = currDecisionNode.AI_HP,
			O_HP = currDecisionNode.O_HP
		};
		if(steps > 0){
			float beta = max(attackDecision, steps -1);
			float alpha = max(defendDecision, steps -1);
			return (alpha > beta ? alpha : beta);
		}else{
			return currDecisionNode.AI_HP - currDecisionNode.O_HP;
		}

		
		//will be last node. calculate heurisitic
	}
	private float max(DecisionNode currDecisionNode, int steps){
		if(currDecisionNode.state == State.attacking && currDecisionNode.otherIsVisible){
			SelfG.shoot();
			SelfG.patrol();
			currDecisionNode.O_HP -= Random.value * 5 + 10;
		}else if(currDecisionNode.state == State.attacking && !currDecisionNode.otherIsVisible){
			SelfG.patrol();
		}else{
			SelfG.takeCover();
		}
		DecisionNode attackDecision = new DecisionNode(){
			previousLocation1 = SelfG.transform.position,
			previousLocation2 = otherG.transform.position,
			state = State.attacking,
			otherIsVisible = isOtherVisible(),
			AI_HP = currDecisionNode.AI_HP,
			O_HP = currDecisionNode.O_HP
		};
		DecisionNode defendDecision = new DecisionNode(){
			previousLocation1 = SelfG.transform.position,
			previousLocation2 = other.transform.position,
			state = State.defending,
			otherIsVisible = isOtherVisible(),
			AI_HP = currDecisionNode.AI_HP,
			O_HP = currDecisionNode.O_HP
		};
		if(steps > 0){
			float alpha = min(attackDecision, steps -1);
			float beta = min(defendDecision, steps -1);
			return (alpha < beta ? alpha : beta);
		}else{
			return currDecisionNode.AI_HP - currDecisionNode.O_HP;
		}
	}

	bool isOtherVisible(){
		RaycastHit hit;
		if(Physics.Raycast(SelfG.transform.position,otherG.transform.position - SelfG.transform.position, out hit)){
			if(hit.collider.gameObject.tag == "otherFuture"){
				return true;
			}else{
				return false;
			}
		}
		return false;
	}

}
