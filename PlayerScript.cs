Transform trans;

public GameObject gameManager;
public List<GameObject> diedObj = new List<GameObject>();
BaseScript baseSc;

public int number;

int process;

void Awake()
    {
        baseSc = gameManager.GetComponent<BaseScript>();
        trans = gameObject.GetComponent<Transform>();

        process = 0;
    }

void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < 28; i++) {
            if (other.gameObject == baseSc.detecter_objs[i] && process == i) {
                process++;
                baseSc.currentProcess[number] = i;
                if (baseSc.playerGenes[number].angleGene[i] != -1) {
                    trans.eulerAngles += new Vector3(0, -baseSc.playerGenes[number].angleGene[i]);
                }
                if (baseSc.playerGenes[number].boostGene[i]) {
                    baseSc.boostTimer[number] += 1.5f;
                }
            }
        }
        if (other.gameObject == baseSc.detecter_objs[28]) {
            process++;
            baseSc.currentProcess[number] = 29;
            baseSc.check_success++;
        }
        if (diedObj.Contains(other.gameObject)) {
            baseSc.isSurvived[number] = false;
            baseSc.player_objs[number].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }