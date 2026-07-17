using UnityEngine;

public class Checkpoints : MonoBehaviour
{

    Vector2 CheckpointPos = new Vector2(0,0);
    [SerializeField]
    GameObject player;
    public int checkpointID;


    public void LoadCheckpoint()
    {
        player.transform.position = CheckpointPos;    
    }

    public void SetCheckpoint(Vector2 newpos)
    {
        CheckpointPos = newpos; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
