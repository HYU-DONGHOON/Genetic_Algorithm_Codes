public class PlayerGene
{
    public bool[] boostGene = new bool[28];
    public float[] angleGene = new float[28];

    public PlayerGene()
    {
        int boostNumber = 5;
        while (boostNumber > 0) {
            int ran = UnityEngine.Random.Range(0, 28);
            if (boostGene[ran] == false) {
                boostGene[ran] = true;
                boostNumber--;
            }
        }
        for (int i = 0; i <= 1; i++) {
            angleGene[i] = -1;
        }
        for (int i = 2; i <= 12; i++) {
            angleGene[i] = UnityEngine.Random.Range(-80f, 80f);
        }
        for (int i = 13; i <= 15; i++) {
            angleGene[i] = -1;
        }
        for (int i = 16; i <= 26; i++) {
            angleGene[i] = UnityEngine.Random.Range(-80f, 80f);
        }
        for (int i = 27; i <= 27; i++) {
            angleGene[i] = -1;
        }
    }
}

public TextMeshProUGUI generation_tmp;
public TextMeshProUGUI mutationProbability_tmp;

public GameObject playerObj;
public GameObject detecters;

public List<GameObject> player_objs = new List<GameObject>();
public List<GameObject> detecter_objs = new List<GameObject>();

public List<PlayerGene> playerGenes = new List<PlayerGene>();

List<float> lifeTime = new List<float>();

public List<int> currentProcess = new List<int>();
public List<float> boostTimer = new List<float>();
public List<bool> isSurvived = new List<bool>();

int unitNumberOfGeneration;
int generation;

int mutationProbability;
int counts_of_trials;
public int check_success;

public GameObject playerObj;
public GameObject detecters;

public List<GameObject> player_objs = new List<GameObject>();
public List<GameObject> detecter_objs = new List<GameObject>();

void Awake()
    {
        for (int i = 0; i < 29; i++) {
            detecter_objs.Add(detecters.transform.GetChild(i).gameObject);
        }
    }

void GenerateRandomGenes(int unitNumber)
    {
        playerGenes.Clear();
        for (int i = 0; i < unitNumber; i++) {
            playerGenes.Add(new PlayerGene());
        }
    }

void GeneratePlayersObj(int unitNumber)
    {
        lifeTime.Clear();
        currentProcess.Clear();
        boostTimer.Clear();
        isSurvived.Clear();
        for (int i = 0; i < unitNumber; i++) {
            player_objs.Add(Instantiate(playerObj));
            player_objs[player_objs.Count - 1].GetComponent<PlayerScript>().number = i;
            player_objs[player_objs.Count - 1].SetActive(true);
            lifeTime.Add(0);
            currentProcess.Add(-1);
            boostTimer.Add(0);
            isSurvived.Add(true);
        }
    }

void Start()
    {
        generation = 1;
        unitNumberOfGeneration = 400;
        GenerateRandomGenes(unitNumberOfGeneration);
        GeneratePlayersObj(unitNumberOfGeneration);
        generation_tmp.text = "Generation: " + generation;

        mutationProbability = 1;
        counts_of_trials = 1;
        check_success = 0;

        mutationProbability_tmp.text = mutationProbability_tmp.text = "mutationProbability: " + mutationProbability.ToString() + "%";

        Time.timeScale = 3;
    }

void Update()
    {
        for (int i = 0; i < playerGenes.Count; i++) {
            if (boostTimer[i] > 0) {
                boostTimer[i] -= Time.deltaTime;
            } else {
                boostTimer[i] = 0;
            }
            if (isSurvived[i]) {
                lifeTime[i] += Time.deltaTime;
            }
        }

        if (!isSurvived.Contains(true)) {
            DevelopGene();
        }

        if (mutationProbability <= 20) {
            if (check_success > 15) {
                check_success = 0;
                SaveBestGene();
                counts_of_trials++;
                if (counts_of_trials > 100) {
                    counts_of_trials = 1;
                    mutationProbability++;
                    mutationProbability_tmp.text = mutationProbability_tmp.text = "mutationProbability: " + mutationProbability.ToString() + "%";
                }
                generation = 1;
                for (int i = 0; i < player_objs.Count; i++) {
                    Destroy(player_objs[i]);
                }
                player_objs.Clear();
                GenerateRandomGenes(unitNumberOfGeneration);
                GeneratePlayersObj(unitNumberOfGeneration);
                generation_tmp.text = "Generation: " + generation;
            }
        }
    }

void PropelPlayer()
    {
        Rigidbody rigid;
        Transform player_trans;
        Transform dir_trans;
        for (int i = 0; i < playerGenes.Count; i++) {
            if (isSurvived[i]) {
                rigid = player_objs[i].GetComponent<Rigidbody>();
                player_trans = player_objs[i].GetComponent<Transform>();
                dir_trans = player_objs[i].transform.GetChild(0).GetComponent<Transform>();
                Vector3 dir = dir_trans.position - player_trans.position;
                if (boostTimer[i] > 0) {
                    rigid.AddForce(dir.normalized * (550 + 150));
                } else {
                    rigid.AddForce(dir.normalized * 550);
                }
            }
        }
    }

void FixedUpdate()
    {
        PropelPlayer();
    }

void DevelopGene()
    {
        for (int i = 0; i < player_objs.Count; i++) {
            Destroy(player_objs[i]);
        }
        player_objs.Clear();
        //Sorting_Genes
        sorted_process.Clear();
        sorted_playerGenes.Clear();
        sorted_lifeTime.Clear();
        for (int i = 0; i < unitNumberOfGeneration; i++) {
            if (sorted_playerGenes.Count == 0) {
                sorted_playerGenes.Add(playerGenes[i]);
                sorted_lifeTime.Add(lifeTime[i]);
                sorted_process.Add(currentProcess[i]);
            } else {
                for (int k = 0; k < sorted_process.Count; k++) {
                    if (currentProcess[i] >= sorted_process[k]) {
                        sorted_playerGenes.Insert(k, playerGenes[i]);
                        sorted_lifeTime.Insert(k, lifeTime[i]);
                        sorted_process.Insert(k, currentProcess[i]);
                        break;
                    } else {
                        if (k == sorted_playerGenes.Count - 1) {
                            sorted_playerGenes.Add(playerGenes[i]);
                            sorted_lifeTime.Add(lifeTime[i]);
                            sorted_process.Add(currentProcess[i]);
                            break;
                        }
                    }
                }
            }
        }
        //Develop
        for (int i = 0; i < 4; i++) {
            playerGenes[i] = sorted_playerGenes[i];
        }
        for (int i = 4; i < unitNumberOfGeneration; i++) {
            //SelectParent
            int parent1_num = UnityEngine.Random.Range(0, 4);
            int parent2_num = UnityEngine.Random.Range(0, 4);
            while (parent1_num == parent2_num) {
                parent2_num = UnityEngine.Random.Range(0, 4);
            }
            bool isDeveloping = true;
            while (isDeveloping) {
                PlayerGene newPlayerGene = new PlayerGene();
                List<int> randomNumbers = new List<int>();
                for (int k = 0; k < 28; k++) {
                    newPlayerGene.boostGene[k] = false;
                }
                //BoostGene
                int coincideGene = 0;
                for (int k = 0; k < 28; k++) {
                    if (playerGenes[parent1_num].boostGene[k] && playerGenes[parent2_num].boostGene[k]) {
                        newPlayerGene.boostGene[k] = playerGenes[parent1_num].boostGene[k];
                        coincideGene++;
                    }
                }
                while (randomNumbers.Count < 5 - coincideGene) {
                    int temp = UnityEngine.Random.Range(0, (5 - coincideGene) * 2);
                    if (!randomNumbers.Contains(temp)) {
                        randomNumbers.Add(temp);
                    }
                }
                randomNumbers.Sort();
                int currentNum = 0;
                for (int k = 0; k < 28; k++) {
                    if (playerGenes[parent1_num].boostGene[k] && !playerGenes[parent2_num].boostGene[k]) {
                        if (randomNumbers.Contains(currentNum)) {
                            newPlayerGene.boostGene[k] = playerGenes[parent1_num].boostGene[k];
                            randomNumbers.RemoveAt(0);
                        }
                        currentNum++;
                    } else if (!playerGenes[parent1_num].boostGene[k] && playerGenes[parent2_num].boostGene[k]) {
                        if (randomNumbers.Contains(currentNum)) {
                            newPlayerGene.boostGene[k] = playerGenes[parent2_num].boostGene[k];
                            randomNumbers.RemoveAt(0);
                        }
                        currentNum++;
                    }
                }
                //AngleGene
                for (int k = 0; k < 28; k++) {
                    if (UnityEngine.Random.Range(0, 100) < mutationProbability) {
                        //MakeMutation
                        if (playerGenes[parent1_num].angleGene[k] == -1) {
                            newPlayerGene.angleGene[k] = -1;
                        } else {
                            newPlayerGene.angleGene[k] = UnityEngine.Random.Range(-80f, 80f);
                        }
                    } else {
                        //Heredity
                        if (UnityEngine.Random.Range(0, 2) == 0) {
                            newPlayerGene.angleGene[k] = playerGenes[parent1_num].angleGene[k];
                        } else {
                            newPlayerGene.angleGene[k] = playerGenes[parent2_num].angleGene[k];
                        }
                    }
                }
                if (!playerGenes.Contains(newPlayerGene)) {
                    playerGenes[i] = newPlayerGene;
                    isDeveloping = false;
                }
            }
        }
        GeneratePlayersObj(unitNumberOfGeneration);
        generation++;
        generation_tmp.text = "Generation: " + generation;
    }

void SaveBestGene()
    {
        PlayerGeneSaveFIle data = new PlayerGeneSaveFIle();
        for (int i = 0; i < 4; i++) {
            data.bestPlayerGenes[i] = playerGenes[i];
        }
        data.generation = generation;
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.dataPath + "/Resources/Results/GeneFile (mutationProbability_" + mutationProbability + "%, No." + counts_of_trials + ").json", jsonData);
        Debug.Log("Save");
    }