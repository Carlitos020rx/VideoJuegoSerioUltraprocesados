#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class GeneradorAlimentosDificiles
{
    const string CARPETA = "Assets/NIVEL1/Alimentos";

    static SelloNutricional CargarSello(string nombreArchivo)
    {
        var guids = AssetDatabase.FindAssets("t:SelloNutricional");
        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            var s = AssetDatabase.LoadAssetAtPath<SelloNutricional>(path);
            if (s != null && s.name == nombreArchivo) return s;
        }
        Debug.LogWarning("[Generador] No se encontro el sello: " + nombreArchivo);
        return null;
    }

    [MenuItem("UltraProcessed/Generar Alimentos Dificiles")]
    public static void Generar()
    {
        if (!AssetDatabase.IsValidFolder("Assets/NIVEL1"))
            AssetDatabase.CreateFolder("Assets", "NIVEL1");
        if (!AssetDatabase.IsValidFolder(CARPETA))
            AssetDatabase.CreateFolder("Assets/NIVEL1", "Alimentos");

        var sodio  = CargarSello("Sello_ExcesoSodio");
        var gsat   = CargarSello("Sello_ExcesoGrasasSaturadas");
        var azucar = CargarSello("Sello_ExcesoAzucares");
        var edulc  = CargarSello("Sello_ContieneEdulcorantes");

        string dMSG  = "Glutamato monosodico (MSG). Comportamientos depresivos y ansiosos.";
        string dTit  = "Colorante. Neuroinflamacion, daña neuronas dopaminergicas.";
        string dBPA  = "Migrante del empaque. Disruptor de sistemas de estres; ligado a estados ansiosos y depresivos.";
        string dCarb = "Emulsionante. Altera microbiota intestinal, genera inflamacion.";
        string dSacar = "Edulcorante. Desregula dopamina, norepinefrina y serotonina.";
        string dSucra = "Edulcorante. Altera metabolitos del triptofano, precursor de serotonina.";
        string dAspar = "Edulcorante. Desregula dopamina, norepinefrina y serotonina.";

        // 1. Papas de paquete -> MSG + Dioxido de titanio
        CrearDificil("AlimentoDificil_PapasPaquete", "Papas de Paquete", "Pasabocas",
            new List<string> { "Papa", "Aceite vegetal", "Sal", "Glutamato monosodico - MSG", "Dioxido de titanio", "Especias" },
            new List<string[]> { new[] { "Glutamato monosodico - MSG", dMSG }, new[] { "Dioxido de titanio", dTit } },
            new List<string[]> { new[]{"Porcion","100g"}, new[]{"Calorias (kcal)","536"}, new[]{"Grasa total","35g"}, new[]{"Grasa saturada","12g"}, new[]{"Grasa trans","0g"}, new[]{"Carbohidratos","53g"}, new[]{"Azucares","1g"}, new[]{"Fibra","4g"}, new[]{"Proteina","6g"}, new[]{"Sodio","525mg"} },
            new List<SelloNutricional> { sodio, gsat });

        // 2. Salchichas -> MSG + Bisfenol A
        CrearDificil("AlimentoDificil_Salchichas", "Salchichas", "Carne procesada",
            new List<string> { "Carne de cerdo", "Agua", "Sal", "Glutamato monosodico - MSG", "Bisfenol A", "Especias" },
            new List<string[]> { new[] { "Glutamato monosodico - MSG", dMSG }, new[] { "Bisfenol A", dBPA } },
            new List<string[]> { new[]{"Porcion","100g"}, new[]{"Calorias (kcal)","301"}, new[]{"Grasa total","27g"}, new[]{"Grasa saturada","10g"}, new[]{"Grasa trans","0g"}, new[]{"Carbohidratos","3g"}, new[]{"Azucares","1g"}, new[]{"Fibra","0g"}, new[]{"Proteina","12g"}, new[]{"Sodio","870mg"} },
            new List<SelloNutricional> { sodio, gsat });

        // 3. Galletas rellenas -> Carboximetilcelulosa + Sacarina
        CrearDificil("AlimentoDificil_GalletasRellenas", "Galletas Rellenas", "Pasabocas dulce",
            new List<string> { "Harina de trigo", "Azucar", "Carboximetilcelulosa", "Sacarina", "Cacao", "Sal" },
            new List<string[]> { new[] { "Carboximetilcelulosa", dCarb }, new[] { "Sacarina", dSacar } },
            new List<string[]> { new[]{"Porcion","100g"}, new[]{"Calorias (kcal)","480"}, new[]{"Grasa total","20g"}, new[]{"Grasa saturada","8g"}, new[]{"Grasa trans","0g"}, new[]{"Carbohidratos","70g"}, new[]{"Azucares","38g"}, new[]{"Fibra","2g"}, new[]{"Proteina","5g"}, new[]{"Sodio","210mg"} },
            new List<SelloNutricional> { azucar, edulc });

        // 4. Bebida energetica -> Sucralosa + Aspartame
        CrearDificil("AlimentoDificil_BebidaEnergetica", "Bebida Energetica", "Bebida",
            new List<string> { "Agua carbonatada", "Azucar", "Cafeina", "Sucralosa", "Aspartame", "Acido citrico" },
            new List<string[]> { new[] { "Sucralosa", dSucra }, new[] { "Aspartame", dAspar } },
            new List<string[]> { new[]{"Porcion","100ml"}, new[]{"Calorias (kcal)","45"}, new[]{"Grasa total","0g"}, new[]{"Grasa saturada","0g"}, new[]{"Grasa trans","0g"}, new[]{"Carbohidratos","11g"}, new[]{"Azucares","11g"}, new[]{"Fibra","0g"}, new[]{"Proteina","0g"}, new[]{"Sodio","100mg"} },
            new List<SelloNutricional> { azucar, edulc });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Generador] 4 alimentos dificiles regenerados con ingredientes de la lista");
    }

    static void CrearDificil(string archivo, string nombre, string categoria,
        List<string> ingredientes, List<string[]> peligrosos, List<string[]> nutri, List<SelloNutricional> sellos)
    {
        var a = ScriptableObject.CreateInstance<AlimentoDificilData>();
        a.nombre = nombre;
        a.categoria = categoria;
        a.ingredientes = new List<string>(ingredientes);
        a.esUltraprocesado = true;
        a.ingredientesPeligrosos = new List<IngredientePeligroso>();
        foreach (var p in peligrosos)
            a.ingredientesPeligrosos.Add(new IngredientePeligroso { nombre = p[0], descripcion = p[1] });
        a.infoNutricional = new List<DatoNutricional>();
        foreach (var d in nutri)
            a.infoNutricional.Add(new DatoNutricional { nombre = d[0], valor = d[1] });
        a.sellosCorrectos = new List<SelloNutricional>();
        foreach (var s in sellos)
            if (s != null) a.sellosCorrectos.Add(s);

        string ruta = CARPETA + "/" + archivo + ".asset";
        var ex = AssetDatabase.LoadAssetAtPath<AlimentoDificilData>(ruta);
        if (ex != null) AssetDatabase.DeleteAsset(ruta);
        AssetDatabase.CreateAsset(a, ruta);
    }
}
#endif
