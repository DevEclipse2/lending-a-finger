using UnityEngine;

public class Checkpoints : MonoBehaviour
{

    Vector2 CheckpointPos;
    [SerializeField]
    Transform player;
    public int checkpointID;


    public void LoadCheckpoint()
    {
        player.position = CheckpointPos;    
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
