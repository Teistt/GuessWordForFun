using UnityEngine;

[System.Serializable]
public class CardsBP
{
    public int id;
    public string word;
    //Enregistre l'ID de la team qui a gagné cette carte pour chacune des manches
    public int M1_win;
    public int M2_win;
    public int M3_win;
}
