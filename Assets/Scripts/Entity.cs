using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Entity : MonoBehaviour {
	protected float moveSpeed;
	protected float stopSpeed; 

	protected abstract void move();
	protected abstract void move (KeyCode button);
	protected abstract void attack ();
}
