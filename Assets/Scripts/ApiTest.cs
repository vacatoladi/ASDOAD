using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class Receita
{
    public int id = 0;
    public string nome = "";
    public int miseenplace = 0;
    public int cuisson = 0;
    public int montage = 0;
    public string miseenplacetext = "";
    public string cuissontext = "";
    public string montagetext = "";
}

[System.Serializable]
public class RecipeList
{
    public List<Receita> receitas;
}

public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}

public class ApiTest : MonoBehaviour
{
    public UIManager UIManager;

    public string apiUrl = "https://recipes.loca.lt/receitas";

    RecipeList listadeReceitas;

    bool isntFirstTimePlaying = false;

    [Header("UI Elements")]
    public TMP_InputField campoPesquisa;      // arraste seu InputField
    public Transform contentResultados;       // arraste o Content do ScrollView
    public GameObject prefabItem;             // arraste o prefab de item (botão da receita)

    private List<GameObject> itensAtivos = new List<GameObject>();


    void Start()
    {
        isntFirstTimePlaying = PlayerPrefs.GetInt("IsntFirstTime", 0) == 1;
        if (!isntFirstTimePlaying)
        {
            PlayerPrefs.SetInt("IsntFirstTime", 1);
            PlayerPrefs.SetString("JsonOffline", "[{ \"id\": 0,\"nome\": \"Bolo de cenoura\", \"miseenplace\": 15, \"cuisson\": 10, \"montage\": 2,\"miseenplacetext\": \"a\", \"cuissontext\": \"a\", \"montagetext\": \"a\" },{ \"id\": 1, \"nome\": \"Strogonoff\", \"miseenplace\": 15, \"cuisson\": 10, \"montage\": 2,\"miseenplacetext\": \"a\", \"cuissontext\": \"a\", \"montagetext\": \"a\" },{\"id\": 2, \"nome\": \"Cuzcuz\", \"miseenplace\": 15, \"cuisson\": 10, \"montage\": 1,\"miseenplacetext\": \"a\", \"cuissontext\": \"a\", \"montagetext\": \"a\" }]");
        }

        campoPesquisa.onValueChanged.AddListener(FiltrarReceitas);

        StartCoroutine(GetItems());

        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.Save();
    }

    IEnumerator GetItems()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.certificateHandler = new AcceptAllCertificates();
        yield return request.SendWebRequest();

        string json;
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro ao acessar API: " + request.error);
            Debug.Log("Usando API offline");
            json = "{\"receitas\":" + PlayerPrefs.GetString("JsonOffline") + "}";
            listadeReceitas = JsonUtility.FromJson<RecipeList>(json);
        }
        else
        {
            string urljson = request.downloadHandler.text;
            Debug.Log("Resposta da API: " + urljson);
            PlayerPrefs.SetString("JsonOffline", urljson);
            json = "{\"receitas\":" + urljson + "}";
            listadeReceitas = JsonUtility.FromJson<RecipeList>(json);
        }
        
        AtualizarLista(listadeReceitas.receitas);

        //campoPesquisa.onValueChanged.AddListener(FiltrarReceitas);

    }

    void ReturnedId(int i)
    {
        UIManager.SelectedRecipe
            (listadeReceitas.receitas[i].id,
            listadeReceitas.receitas[i].miseenplace,
            listadeReceitas.receitas[i].cuisson,
            listadeReceitas.receitas[i].montage,
            listadeReceitas.receitas[i].miseenplacetext,
            listadeReceitas.receitas[i].cuissontext,
            listadeReceitas.receitas[i].montagetext);
    }

    void FiltrarReceitas(string texto)
    {
        if (listadeReceitas == null || listadeReceitas.receitas == null) return;

        var filtradas = listadeReceitas.receitas.FindAll(r =>
            r.nome.ToLower().Contains(texto.ToLower()));

        AtualizarLista(filtradas);
    }

    void AtualizarLista(List<Receita> receitas)
    {
        // limpa os itens anteriores
        foreach (var item in itensAtivos)
            Destroy(item);
        itensAtivos.Clear();

        // cria os novos
        foreach (var r in receitas)
        {
            GameObject novoItem = Instantiate(prefabItem, contentResultados);
            novoItem.GetComponentInChildren<TMP_Text>().text = r.nome;

            // evento para abrir receita
            novoItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                AbrirReceita(r);
            });

            itensAtivos.Add(novoItem);
        }
    }

    void AbrirReceita(Receita receita)
    {
        Debug.Log("Abrindo receita: " + receita.nome);
        int i = listadeReceitas.receitas.FindIndex(f => f.nome == receita.nome);
        ReturnedId(i);
    }
}