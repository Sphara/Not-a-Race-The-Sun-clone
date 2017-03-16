using UnityEngine;
using System.Collections;

public class GenericPoolableObject : MonoBehaviour {

    public string mName;

	protected virtual void Start() {

    }
	
	protected virtual void FixedUpdate() {

        if (transform.position.z < GlobalSettings.DespawnZone)
            Pool.Destroy(mName, gameObject);
	}

    public virtual void Reset()
    {
        // Reset object state here
    }
}
