using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

enum RecipePart
{
    None,
    MiseEnPlace,
    Cuisson,
    Montage,
    Ended
}

public class UIManager : MonoBehaviour
{
    RecipePart part = RecipePart.None;
    
    public Transform papai;
    public GameObject prefab;
    public GameObject menuTarefas;
    public GameObject menuSearch;
    public TMP_InputField searchField;
    public GameObject searchOptions;

    RectTransform rect;

    public Animator animatorMep;
    public Animator animatorC;
    public Animator animatorM;

    int id;
    int miseenplaceQuantidad;
    int cuissonQuantidad;
    int montageQuantidad;

    int mepCompleted;
    int cCompleted;
    int mCompleted;

    string[] miseenplacePart = new string[20];
    string[] cuissonPart = new string[20];
    string[] montagePart = new string[20];

    GameObject ultimoSelecionado;

    int selecionado;
    bool ativado;

    private void Start()
    {
        rect = papai.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0,0);
    }
    
    void Update()
    {
        GameObject atual = EventSystem.current.currentSelectedGameObject;
        if (ativado)
        {
            float vertical = Input.GetAxisRaw("Vertical");

            if (atual != ultimoSelecionado)
            {
                if (vertical > 0)
                {
                    if(selecionado == 1)
                        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - 160);
                    else 
                        selecionado --;
                }
                else if (vertical < 0)
                {
                    if (selecionado == 4)
                        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + 160);
                    else
                        selecionado ++;
                }
                Debug.Log(selecionado);
                ultimoSelecionado = atual;
            }
            return;
        }
        //TMP_InputField tmp = atual.GetComponent<TMP_InputField>();
        //if (searchField == tmp)
        //{
        //    searchOptions.SetActive(true);
        //}
        //else
        //{
        //    searchOptions.SetActive(false);
        //}
    }

    public void SelectedRecipe(int rid,int mepQ, int cQ, int mQ, string mep, string c, string m)
    {
        ativado = true;
        menuTarefas.SetActive(ativado);
        menuSearch.SetActive(false);
        id = rid;
        miseenplaceQuantidad = mepQ;
        cuissonQuantidad = cQ;
        montageQuantidad = mQ;

        int i = 0;
        foreach (char cc in mep)
        {
            if (cc == '%') i++;
            else miseenplacePart[i] += cc;
        }
        int i2 = 0;
        foreach (char cc in c)
        {
            if (cc == '%') i2++;
            else cuissonPart[i2] += cc;
        }
        int i3 = 0;
        foreach (char cc in m)
        {
            if (cc == '%') i3++;
            else montagePart[i3] += cc;
        }
        MiseEnPlace();
    }

    void MiseEnPlace()
    {
        //animator.SetTrigger("miseenplace");
        part = RecipePart.MiseEnPlace;
        for (int i = 0; i < miseenplaceQuantidad; i++)
        {
            GameObject g = Instantiate(prefab, papai);
            TextMeshProUGUI text = g.GetComponentInChildren<TextMeshProUGUI>();
            Toggle tg = g.GetComponentInChildren<Toggle>();
            tg.onValueChanged.AddListener((bool ativo) => { MiseEnPlaceTask(ativo); });
            if (i == 0){ EventSystem.current.SetSelectedGameObject(tg.gameObject); ultimoSelecionado = tg.gameObject; selecionado = 1;}
            text.text = miseenplacePart[i];
        } 
    }

    void MiseEnPlaceTask(bool b)
    {
        Debug.Log("o botão esta"+b);
        if(b)
        {
            mepCompleted++;
            if (mepCompleted == miseenplaceQuantidad)
            {
                Erase();
                Cuisson();
            }
        }
        else
        {
            mepCompleted--;
        }
    }

    void Cuisson()
    {
        //animator.SetTrigger("cuisson");
        part = RecipePart.Cuisson;
        Debug.Log("foi");
        for (int i = 0; i < cuissonQuantidad; i++)
        {
            GameObject g = Instantiate(prefab, papai);
            TextMeshProUGUI text = g.GetComponentInChildren<TextMeshProUGUI>();
            Toggle tg = g.GetComponentInChildren<Toggle>();
            tg.onValueChanged.AddListener((bool ativo) => { CuissonTask(ativo); });
            if (i == 0) { EventSystem.current.SetSelectedGameObject(tg.gameObject); ultimoSelecionado = tg.gameObject; selecionado = 1; }
            text.text = cuissonPart[i];
        }
        
    }

    void CuissonTask(bool b)
    {
        Debug.Log("o botão esta" + b);
        if (b)
        {
            cCompleted++;
            if (cCompleted == cuissonQuantidad)
            {
                Erase();
                Montage();
            }
        }
        else
        {
            cCompleted--;
        }
    }

    void Montage()
    {
        part = RecipePart.Montage;
        //animator.SetTrigger("montage");
        for (int i = 0; i < montageQuantidad; i++)
        {
            GameObject g = Instantiate(prefab, papai);
            TextMeshProUGUI text = g.GetComponentInChildren<TextMeshProUGUI>();
            Toggle tg = g.GetComponentInChildren<Toggle>();
            tg.onValueChanged.AddListener((bool ativo) => { MontageTask(ativo); });
            if (i == 0) { EventSystem.current.SetSelectedGameObject(tg.gameObject); ultimoSelecionado = tg.gameObject; selecionado = 1; }
            text.text = montagePart[i];
        }
    }

    void MontageTask(bool b)
    {
        Debug.Log("o botão esta" + b);
        if (b)
        {
            mCompleted++;
            if (mCompleted == montageQuantidad)
            {
                Erase();
                part = RecipePart.Ended;
                //animator.SetTrigger("end");
                //
                //Aqui eu vou colocar para aparecer uma tela que vai dizer que a receita está pronta
                //
            }
        }
        else
        {
            mCompleted--;
        }
    }

    void Erase()
    {
        foreach (Transform t in papai)
        {
            Destroy(t.gameObject);
        }
        RectTransform content = papai.GetComponent<RectTransform>();
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0f);
    }

    public void Return()
    {
        Erase();
        switch (part)
        {
            case RecipePart.MiseEnPlace:
                //aqui vou colocar para voltar para barra de pesquisas
                break;
            case RecipePart.Cuisson:
                MiseEnPlace();
                break;
            case RecipePart.Montage:
                Cuisson();
                break;
            case RecipePart.Ended:
                Montage();
                break;
        }
    }

    public void Next()
    {
        Erase();
        switch (part)
        {
            case RecipePart.MiseEnPlace:
                Cuisson();
                break;
            case RecipePart.Cuisson:
                Montage();
                break;
            case RecipePart.Montage:
                //
                //animator.SetTrigger("end");
                //Aqui eu vou colocar para aparecer uma tela que vai dizer que a receita está pronta
                //o mesmo que está na função MontageTask()
                //
                break;
            case RecipePart.Ended:
                //aqui vou colocar para voltar para barra de pesquisas
                break;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    void OnApplicationQuit()
    {
        Application.Quit();
    }
}
