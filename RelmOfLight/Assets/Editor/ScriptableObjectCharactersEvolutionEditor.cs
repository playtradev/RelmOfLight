using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PlaytraGamesLtd;
using System.Linq;

[CustomEditor(typeof(ScriptableObjectCharactersEvolution))]
public class ScriptableObjectCharactersEvolutionEditor : Editor
{
    public ScriptableObjectContainingAllCharsPrefab AllChars;

    ScriptableObjectCharactersEvolution origin;
    bool comboingVisible, costVisible = false, statsEvoVisibile = false, charsVisible = false;
    bool HPVisisble = false, ArmourVisisble = false, EtherVisisble = false, SpeedVisisble = false, LuckVisisble = false, DamageVisisble = false;
    bool miscVisible = false, dropMultiplierVisible = false, etherCostMultiplierVisible = false;
    string tempstring;
    float a, s, d, f, g, h, j, k, l, q;
    Vector2 av, sv, dv, fv, gv, hv, jv, kv, lv, qv;

    SerializedProperty ComboPointWeightPairs, ComboFulfillmentMultiplierCurve;

    private void OnEnable()
    {
        ComboPointWeightPairs = serializedObject.FindProperty("ComboPointWeightPairs");
        ComboFulfillmentMultiplierCurve = serializedObject.FindProperty("ComboFulfillmentMultiplierCurve");
    }

    public override void OnInspectorGUI()
    {
        origin = (ScriptableObjectCharactersEvolution)target;

        serializedObject.Update();
        comboingVisible = EditorGUILayout.Foldout(comboingVisible, "Combo Weighting Info ---------------------------------------------");
        if (comboingVisible)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(ComboPointWeightPairs);
            EditorGUILayout.PropertyField(ComboFulfillmentMultiplierCurve);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        serializedObject.ApplyModifiedProperties();

        costVisible = EditorGUILayout.Foldout(costVisible, "Cost Info ---------------------------------------------");
        if(costVisible)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            EditorGUIUtility.labelWidth = 40;
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i < origin.CostMultiplier.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.CostMultiplier[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.CostMultiplier[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();


            HPVisisble = EditorGUILayout.Foldout(HPVisisble, "HPVisisble Info ---------------------------------------------");
            if (HPVisisble)
            {
                EditorGUIUtility.labelWidth = 100;
                EditorGUILayout.BeginHorizontal();

                origin.HP.BaseCost[0] = EditorGUILayout.IntField("Valley", origin.HP.BaseCost[0]);
                origin.HP.BaseCost[1] = EditorGUILayout.IntField("Mountain", origin.HP.BaseCost[1]);
                origin.HP.BaseCost[2] = EditorGUILayout.IntField("Forest", origin.HP.BaseCost[2]);
                origin.HP.BaseCost[3] = EditorGUILayout.IntField("Desert", origin.HP.BaseCost[3]);

                EditorGUILayout.EndHorizontal();
            }

            ArmourVisisble = EditorGUILayout.Foldout(ArmourVisisble, "ArmourVisisble Info ---------------------------------------------");
            if (ArmourVisisble)
            {
                EditorGUILayout.BeginHorizontal();

                origin.Armour.BaseCost[0] = EditorGUILayout.IntField("Valley", origin.Armour.BaseCost[0]);
                origin.Armour.BaseCost[1] = EditorGUILayout.IntField("Mountain", origin.Armour.BaseCost[1]);
                origin.Armour.BaseCost[2] = EditorGUILayout.IntField("Forest", origin.Armour.BaseCost[2]);
                origin.Armour.BaseCost[3] = EditorGUILayout.IntField("Desert", origin.Armour.BaseCost[3]);

                EditorGUILayout.EndHorizontal();
            }

            EtherVisisble = EditorGUILayout.Foldout(EtherVisisble, "EtherVisisble Info ---------------------------------------------");
            if (EtherVisisble)
            {
                EditorGUILayout.BeginHorizontal();

                origin.Ether.BaseCost[0] = EditorGUILayout.IntField("Valley", origin.Ether.BaseCost[0]);
                origin.Ether.BaseCost[1] = EditorGUILayout.IntField("Mountain", origin.Ether.BaseCost[1]);
                origin.Ether.BaseCost[2] = EditorGUILayout.IntField("Forest", origin.Ether.BaseCost[2]);
                origin.Ether.BaseCost[3] = EditorGUILayout.IntField("Desert", origin.Ether.BaseCost[3]);

                EditorGUILayout.EndHorizontal();
            }

            SpeedVisisble = EditorGUILayout.Foldout(SpeedVisisble, "SpeedVisisble Info ---------------------------------------------");
            if (SpeedVisisble)
            {
                EditorGUILayout.BeginHorizontal();

                origin.Speed.BaseCost[0] = EditorGUILayout.IntField("Valley", origin.Speed.BaseCost[0]);
                origin.Speed.BaseCost[1] = EditorGUILayout.IntField("Mountain", origin.Speed.BaseCost[1]);
                origin.Speed.BaseCost[2] = EditorGUILayout.IntField("Forest", origin.Speed.BaseCost[2]);
                origin.Speed.BaseCost[3] = EditorGUILayout.IntField("Desert", origin.Speed.BaseCost[3]);

                EditorGUILayout.EndHorizontal();
            }

            LuckVisisble = EditorGUILayout.Foldout(LuckVisisble, "LuckVisisble Info ---------------------------------------------");
            if (LuckVisisble)
            {
                EditorGUILayout.BeginHorizontal();

                origin.Luck.BaseCost[0] = EditorGUILayout.IntField("Valley", origin.Luck.BaseCost[0]);
                origin.Luck.BaseCost[1] = EditorGUILayout.IntField("Mountain", origin.Luck.BaseCost[1]);
                origin.Luck.BaseCost[2] = EditorGUILayout.IntField("Forest", origin.Luck.BaseCost[2]);
                origin.Luck.BaseCost[3] = EditorGUILayout.IntField("Desert", origin.Luck.BaseCost[3]);

                EditorGUILayout.EndHorizontal();
            }

            DamageVisisble = EditorGUILayout.Foldout(DamageVisisble, "DamageVisisble Info ---------------------------------------------");
            if (DamageVisisble)
            {
                EditorGUILayout.BeginHorizontal();

                origin.Damage.BaseCost[0] = EditorGUILayout.IntField("Valley", origin.Damage.BaseCost[0]);
                origin.Damage.BaseCost[1] = EditorGUILayout.IntField("Mountain", origin.Damage.BaseCost[1]);
                origin.Damage.BaseCost[2] = EditorGUILayout.IntField("Forest", origin.Damage.BaseCost[2]);
                origin.Damage.BaseCost[3] = EditorGUILayout.IntField("Desert", origin.Damage.BaseCost[3]);

                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;

            if (GUILayout.Button("Print cost"))
            {
                #region HP
                tempstring = "HP \n";
               

                a = origin.HP.BaseCost[0] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];
                q = l * origin.CostMultiplier[9];

                tempstring += "Valley   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "  " + q + "\n";

                a = origin.HP.BaseCost[1] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];
                q = l * origin.CostMultiplier[9];

                tempstring += "Mountain   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "  " + q + "\n";


                a = origin.HP.BaseCost[2] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];
                q = l * origin.CostMultiplier[9];

                tempstring += "Forest   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "  " + q + "\n";

                a = origin.HP.BaseCost[3] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];
                q = l * origin.CostMultiplier[9];

                tempstring += "Desert   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "  " + q + "\n";


                Debug.Log(tempstring);

                #endregion
                #region Armour
                tempstring = "Armour \n";

                a = origin.Armour.BaseCost[0] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Valley   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Armour.BaseCost[1] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Mountain   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Armour.BaseCost[2] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Forest   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Armour.BaseCost[3] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Desert   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";


                Debug.Log(tempstring);

                #endregion
                #region Ether
                tempstring = "Ether \n";



                a = origin.Ether.BaseCost[0] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Valley   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Ether.BaseCost[1] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Mountain   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Ether.BaseCost[2] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Forest   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Ether.BaseCost[3] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Desert   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                Debug.Log(tempstring);
                #endregion
                #region Speed
                tempstring = "Speed \n";

                a = origin.Speed.BaseCost[0] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Valley   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Speed.BaseCost[1] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Mountain   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Speed.BaseCost[2] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Forest   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Speed.BaseCost[3] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Desert   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                Debug.Log(tempstring);
                #endregion
                #region Luck
                tempstring = "Luck \n";



                a = origin.Luck.BaseCost[0] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Valley   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Luck.BaseCost[1] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Mountain   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Luck.BaseCost[2] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Forest   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Luck.BaseCost[3] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Desert   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                Debug.Log(tempstring);
                #endregion
                #region Damage
                tempstring ="Damage \n";

                a = origin.Damage.BaseCost[0] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Valley   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Damage.BaseCost[1] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Mountain   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Damage.BaseCost[2] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Forest   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                a = origin.Damage.BaseCost[3] * origin.CostMultiplier[0];
                s = a * origin.CostMultiplier[1];
                d = s * origin.CostMultiplier[2];
                f = d * origin.CostMultiplier[3];
                g = f * origin.CostMultiplier[4];
                h = g * origin.CostMultiplier[5];
                j = h * origin.CostMultiplier[6];
                k = j * origin.CostMultiplier[7];
                l = k * origin.CostMultiplier[8];

                tempstring +="Desert   " + a + "  " +
                    s + "  " +
                    d + "  " +
                    f + "  " +
                    g + "  " +
                    h + "  " +
                    j + "  " +
                    k + "  " +
                    l + "\n";

                Debug.Log(tempstring);
                #endregion
            }
        }

        statsEvoVisibile = EditorGUILayout.Foldout(statsEvoVisibile, "Stats Evolution Info ---------------------------------------------");
        if (statsEvoVisibile)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            EditorGUIUtility.labelWidth = 40;
            #region HPBase
            EditorGUILayout.LabelField("HP");
            EditorGUILayout.BeginHorizontal();

            for (int i = 1; i < origin.BaseHPStaseCurve.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.BaseHPStaseCurve[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.BaseHPStaseCurve[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.LabelField("HP Regen");
            EditorGUILayout.BeginHorizontal();

            for (int i = 1; i < origin.BaseHPRegenStaseCurve.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.BaseHPRegenStaseCurve[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.BaseHPRegenStaseCurve[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();

            #endregion
            #region ArmourBase
            EditorGUILayout.LabelField("Armour");
            EditorGUILayout.BeginHorizontal();

            for (int i = 1; i < origin.BaseArmourStaseCurve.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.BaseArmourStaseCurve[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.BaseArmourStaseCurve[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Shield  Regen");
            EditorGUILayout.BeginHorizontal();

            for (int i = 1; i < origin.BaseArmourShieldRegenStaseCurve.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.BaseArmourShieldRegenStaseCurve[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.BaseArmourShieldRegenStaseCurve[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();

            #endregion
            #region EtherBase
            EditorGUILayout.LabelField("Ether");
            EditorGUILayout.BeginHorizontal();

            for (int i = 1; i < origin.BaseEtherStaseCurve.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.BaseEtherStaseCurve[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.BaseEtherStaseCurve[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Ether Regen");
            EditorGUILayout.BeginHorizontal();

            for (int i = 1; i < origin.BaseEtherRegenStaseCurve.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.BaseEtherRegenStaseCurve[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.BaseEtherRegenStaseCurve[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();

            #endregion
            #region SpeedBase
            EditorGUILayout.LabelField("Movement Speed");
            EditorGUILayout.BeginHorizontal();

            for (int i = 1; i < origin.BaseSpeedMovementStaseCurve.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.BaseSpeedMovementStaseCurve[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.BaseSpeedMovementStaseCurve[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Agility");
            EditorGUILayout.BeginHorizontal();

            for (int i = 1; i < origin.BaseSpeedAgilityStaseCurve.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.BaseSpeedAgilityStaseCurve[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.BaseSpeedAgilityStaseCurve[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();

            #endregion
        
            #region LuckBase
            EditorGUILayout.LabelField("Critical Hit");
            EditorGUILayout.BeginHorizontal();

            for (int i = 1; i < origin.BaseLuckCCStaseCurve.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.BaseLuckCCStaseCurve[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.BaseLuckCCStaseCurve[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("SigilDropBonus");
            EditorGUILayout.BeginHorizontal();

            for (int i = 1; i < origin.BaseLuckSigilDropBonusStaseCurve.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.BaseLuckSigilDropBonusStaseCurve[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.BaseLuckSigilDropBonusStaseCurve[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();
            #endregion
            #region DamageBase
            EditorGUILayout.LabelField("Weak Damage");
            EditorGUILayout.BeginHorizontal();

            for (int i = 1; i < origin.BaseDamageWeakStaseCurve.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.BaseDamageWeakStaseCurve[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.BaseDamageWeakStaseCurve[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Strong Damage");
            EditorGUILayout.BeginHorizontal();

            for (int i = 1; i < origin.BaseDamageStrongStaseCurve.Length; i++)
            {
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                origin.BaseDamageStrongStaseCurve[i] = EditorGUILayout.FloatField("Lvl" + (i + 1).ToString(), origin.BaseDamageStrongStaseCurve[i], GUILayout.Width(100));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

            }
            EditorGUILayout.EndHorizontal();

            #endregion

            charsVisible = EditorGUILayout.Foldout(charsVisible, "Chars Info ---------------------------------------------");
            if (charsVisible)
            {
                EditorGUIUtility.labelWidth = 200;
                var list = origin.Characters;
                int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Chars", origin.Characters.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(null);

                for (int i = 0; i < list.Count; i++)
                {
                    WriteChar(list[i]);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }
            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        EditorUtility.SetDirty(origin);
    }

    CharacterInfoScript charI;
    public void WriteChar(CharacterEvolutionClass charToShow)
    {
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        charToShow.Show = EditorGUILayout.Foldout(charToShow.Show, charToShow.CharacterName + " Info ---------------------------------------------");
        if (charToShow.Show)
        {
            charI = AllChars.Prefabs.Where(r => r.CharacterID == charToShow.CharacterName).FirstOrDefault();

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            EditorGUIUtility.labelWidth = 200;

            charToShow.CharacterName = (CharacterNameType)EditorGUILayout.EnumPopup("Char Name", charToShow.CharacterName);
            EditorGUIUtility.labelWidth = 40;

            EditorGUILayout.LabelField("HP: ");
            EditorGUILayout.BeginHorizontal();


            for (int i = 1; i < origin.BaseHPStaseCurve.Length; i++)
            {

                charToShow.HPLevel[i] = EditorGUILayout.IntField("Lvl" + (i + 1).ToString(), charToShow.HPLevel[i], GUILayout.Width(100));

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("HP Regen: ");
            EditorGUILayout.BeginHorizontal();


            for (int i = 1; i < origin.BaseHPRegenStaseCurve.Length; i++)
            {

                charToShow.HPRegenLevel[i] = EditorGUILayout.IntField("Lvl" + (i + 1).ToString(), charToShow.HPRegenLevel[i], GUILayout.Width(100));

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Armour: ");
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i < origin.BaseArmourStaseCurve.Length; i++)
            {
                charToShow.ArmourLevel[i] = EditorGUILayout.IntField("Lvl" + (i + 1).ToString(), charToShow.ArmourLevel[i], GUILayout.Width(100));

            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Shield Regen: ");
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i < origin.BaseArmourShieldRegenStaseCurve.Length; i++)
            {
                charToShow.ShieldRegenLevel[i] = EditorGUILayout.IntField("Lvl" + (i + 1).ToString(), charToShow.ShieldRegenLevel[i], GUILayout.Width(100));

            }

            EditorGUILayout.EndHorizontal();



            EditorGUILayout.LabelField("Ether: ");
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i < origin.BaseEtherStaseCurve.Length; i++)
            {
                charToShow.EtherLevel[i] = EditorGUILayout.IntField("Lvl" + (i + 1).ToString(), charToShow.EtherLevel[i], GUILayout.Width(100));

            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.LabelField("Ether Regen: ");
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i < origin.BaseEtherRegenStaseCurve.Length; i++)
            {
                charToShow.EtherRegenLevel[i] = EditorGUILayout.IntField("Lvl" + (i + 1).ToString(), charToShow.EtherRegenLevel[i], GUILayout.Width(100));

            }
            EditorGUILayout.EndHorizontal();

       



            EditorGUILayout.LabelField("Movement Speed: ");
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i < origin.BaseSpeedMovementStaseCurve.Length; i++)
            {
                charToShow.SpeedMovementLevel[i] = EditorGUILayout.IntField("Lvl" + (i + 1).ToString(), charToShow.SpeedMovementLevel[i], GUILayout.Width(100));

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Agility chances: ");
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i < origin.BaseSpeedAgilityStaseCurve.Length; i++)
            {
                charToShow.AgilityLevel[i] = EditorGUILayout.IntField("Lvl" + (i + 1).ToString(), charToShow.AgilityLevel[i], GUILayout.Width(100));

            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.LabelField("Sigil Drop Bonus: ");
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i < origin.BaseLuckSigilDropBonusStaseCurve.Length; i++)
            {
                charToShow.SigilDropBonusLevel[i] = EditorGUILayout.IntField("Lvl" + (i + 1).ToString(), charToShow.SigilDropBonusLevel[i], GUILayout.Width(100));

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Crit chances: ");
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i < origin.BaseLuckCCStaseCurve.Length; i++)
            {
                charToShow.CritCLevel[i] = EditorGUILayout.IntField("Lvl" + (i + 1).ToString(), charToShow.CritCLevel[i], GUILayout.Width(100));

            }
            EditorGUILayout.EndHorizontal();

        


            EditorGUILayout.LabelField("Weak Damage: ");
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i < origin.BaseDamageWeakStaseCurve.Length; i++)
            {
                charToShow.DamageWeakLevel[i] = EditorGUILayout.IntField("Lvl" + (i + 1).ToString(), charToShow.DamageWeakLevel[i], GUILayout.Width(100));

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Strong Damage: ");
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i < origin.BaseDamageStrongStaseCurve.Length; i++)
            {
                charToShow.DamageStrongLevel[i] = EditorGUILayout.IntField("Lvl" + (i + 1).ToString(), charToShow.DamageStrongLevel[i], GUILayout.Width(100));

            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
        }
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }


    private string StampStatsValues(float val, float[] baseCurve, int[] lvls)
    {
        a = (val / 100) * (baseCurve[0] + ((baseCurve[0] / 100) * lvls[0]));
        s = a + (a / 100) * (baseCurve[1] + ((baseCurve[1] / 100) * lvls[1]));
        d = s + (s / 100) * (baseCurve[2] + ((baseCurve[2] / 100) * lvls[2]));
        f = d + (d / 100) * (baseCurve[3] + ((baseCurve[3] / 100) * lvls[3]));
        g = f + (f / 100) * (baseCurve[4] + ((baseCurve[4] / 100) * lvls[4]));
        h = g + (g / 100) * (baseCurve[5] + ((baseCurve[5] / 100) * lvls[5]));
        j = h + (h / 100) * (baseCurve[6] + ((baseCurve[6] / 100) * lvls[6]));
        k = j + (j / 100) * (baseCurve[7] + ((baseCurve[7] / 100) * lvls[7]));
        l = k + (k / 100) * (baseCurve[8] + ((baseCurve[8] / 100) * lvls[8]));
        q = l + (l / 100) * (baseCurve[9] + ((baseCurve[9] / 100) * lvls[9]));

        return  a + "  " +
         s + "  " +
         d + "  " +
         f + "  " +
         g + "  " +
         h + "  " +
         j + "  " +
         k + "  " +
         l + "  " + q + "\n";
    }
    private string StampStatsValues(Vector2 val, float[] baseCurve, int[] lvls)
    {
        av = (val / 100) * (baseCurve[0] + ((baseCurve[0] / 100) * lvls[0]));
        sv = av + (av / 100) * (baseCurve[1] + ((baseCurve[1] / 100) * lvls[1]));
        dv = sv + (sv / 100) * (baseCurve[2] + ((baseCurve[2] / 100) * lvls[2]));
        fv = dv + (dv / 100) * (baseCurve[3] + ((baseCurve[3] / 100) * lvls[3]));
        gv = fv + (fv / 100) * (baseCurve[4] + ((baseCurve[4] / 100) * lvls[4]));
        hv = gv + (gv / 100) * (baseCurve[5] + ((baseCurve[5] / 100) * lvls[5]));
        jv = hv + (hv / 100) * (baseCurve[6] + ((baseCurve[6] / 100) * lvls[6]));
        kv = jv + (jv / 100) * (baseCurve[7] + ((baseCurve[7] / 100) * lvls[7]));
        lv = kv + (kv / 100) * (baseCurve[8] + ((baseCurve[8] / 100) * lvls[8]));
        qv = lv + (lv / 100) * (baseCurve[9] + ((baseCurve[9] / 100) * lvls[9]));

        return av + "  " +
         sv + "  " +
         dv + "  " +
         fv + "  " +
         gv + "  " +
         hv + "  " +
         jv + "  " +
         kv + "  " +
         lv + "  " + qv + "\n";
    }
}
