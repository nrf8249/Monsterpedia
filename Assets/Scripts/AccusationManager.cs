//using UnityEngine;

//public class AccusationManager : MonoBehaviour
//{
//    public static AccusationManager Instance;
//    //public Dialogue accusationBox;

//    private void Awake()
//    {
//        if (Instance == null) Instance = this;
//        else Destroy(gameObject);

//        accusationBox = GameObject.Find("AccusationBox").GetComponent<Dialogue>();
//        accusationBox.gameObject.SetActive(false);
//    }

//    public void StartAccusation(string[] lines)
//    {
//        accusationBox.lines = lines;
//        accusationBox.StartDialogue();
//        Debug.Log("Starting dialogue...");
//    }

//    public void StopAccusation()
//    {
//        accusationBox.StopDialogue();
//    }
//}
