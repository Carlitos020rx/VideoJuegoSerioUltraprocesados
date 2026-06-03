#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// Script de editor para generar los 4 alimentos faciles automaticamente.
// Ejecutar desde el menu: UltraProcessed/Generar Alimentos Faciles
public static class GeneradorAlimentosFaciles
{
    const string CARPETA = "Assets/NIVEL1/Alimentos";

    [MenuItem("UltraProcessed/Generar Alimentos Faciles")]
    public static void Generar()
    {
        // Asegurar carpeta
        if (!AssetDatabase.IsValidFolder("Assets/NIVEL1"))
            AssetDatabase.CreateFolder("Assets", "NIVEL1");
        if (!AssetDatabase.IsValidFolder(CARPETA))
            AssetDatabase.CreateFolder("Assets/NIVEL1", "Alimentos");

        // ── Alimento 1: 1 peligroso (Dioxido de titanio) ──
        CrearAlimento(
            "Alimento_ChicleBlanco",
            "Chicle Mentolado",
            "Goma de mascar",
            new List<string> { "Goma base", "Agua", "Menta natural", "Dioxido de titanio", "Jarabe de glucosa" },
            new List<(string, string)> {
                ("Dioxido de titanio", "Colorante usado por su blancura. Se ha vinculado con neuroinflamacion y daño a las neuronas dopaminergicas.")
            },
            true);

        // ── Alimento 2: 2 peligrosos (Aspartame + Sucralosa) ──
        CrearAlimento(
            "Alimento_GaseosaLight",
            "Gaseosa Light",
            "Bebida",
            new List<string> { "Agua carbonatada", "Aspartame", "Sucralosa", "Acido citrico", "Saborizante natural" },
            new List<(string, string)> {
                ("Aspartame", "Edulcorante artificial. Puede desregular la dopamina, norepinefrina y serotonina, neurotransmisores del animo."),
                ("Sucralosa", "Edulcorante artificial. Altera los metabolitos del triptofano, precursor de la serotonina.")
            },
            true);

        // ── Alimento 3: 2 peligrosos (Carboximetilcelulosa + Polisorbato-80) ──
        CrearAlimento(
            "Alimento_HeladoCremoso",
            "Helado Cremoso",
            "Postre",
            new List<string> { "Leche", "Azucar", "Carboximetilcelulosa", "Polisorbato-80", "Vainilla" },
            new List<(string, string)> {
                ("Carboximetilcelulosa", "Emulsionante. Altera la microbiota intestinal y puede generar inflamacion."),
                ("Polisorbato-80", "Emulsionante. Altera la microbiota intestinal y puede generar inflamacion.")
            },
            true);

        // ── Alimento 4: 3 peligrosos (GMS + Bisfenol A + Dioxido de titanio) ──
        CrearAlimento(
            "Alimento_SopaInstantanea",
            "Sopa Instantanea",
            "Comida lista",
            new List<string> { "Fideos", "Sal", "Glutamato monosodico", "Bisfenol A", "Dioxido de titanio", "Vegetales deshidratados" },
            new List<(string, string)> {
                ("Glutamato monosodico", "Potenciador del sabor (E-621). Asociado a comportamientos depresivos y ansiosos en estudios."),
                ("Bisfenol A", "Migrante del empaque. Disruptor de los sistemas de estres, ligado a estados ansiosos y depresivos."),
                ("Dioxido de titanio", "Colorante usado por su blancura. Se ha vinculado con neuroinflamacion y daño a las neuronas dopaminergicas.")
            },
            true);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Generador] 4 alimentos faciles creados en " + CARPETA);
    }

    static void CrearAlimento(string archivo, string nombre, string categoria,
        List<string> ingredientes, List<(string nombre, string desc)> peligrosos, bool esUltra)
    {
        var alimento = ScriptableObject.CreateInstance<AlimentoData>();
        alimento.nombre = nombre;
        alimento.categoria = categoria;
        alimento.ingredientes = new List<string>(ingredientes);
        alimento.esUltraprocesado = esUltra;
        alimento.ingredientesPeligrosos = new List<IngredientePeligroso>();
        foreach (var p in peligrosos)
        {
            alimento.ingredientesPeligrosos.Add(new IngredientePeligroso {
                nombre = p.nombre,
                descripcion = p.desc
            });
        }

        string ruta = CARPETA + "/" + archivo + ".asset";
        AssetDatabase.CreateAsset(alimento, ruta);
    }
}
#endif
