//Nicolas Alarcón
//Joseph Shakalo
//Emiliano Romero

//Game controller script to manage flow of the game
//04 - 12 - 2024


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class GameController_DeathRace : MonoBehaviour
{
    //Variables, objects and classes
    public PlayerData playerData;
    int game_id;
    int player_id;
    
    int reward_id;
    int level;
    int turn = 0;
    int energy;
    bool coroutineFinished1;
    bool coroutineFinished2;
    bool coroutineFinished3;
    bool coroutineFinished4;
    bool checkStartGame;
    bool gameStarted;
    int energyRival;
    int turnDuration = 10;
    float playerCurrentPosition = -8;
    float rivalCurrentPosition = -8;
    public GameObject carPrefab;

    public GameObject playerCar;

    public GameObject rivalCar;

    public GameObject playerHealthBar;

    public GameObject rivalHealthBar;

    public GameObject background;

    public Car player;

    public Car rival;

    int nextTurnPlayerVelocity;
    int playerVelocity;
    int playerHealth;
    int nextTurnRivalVelocity;
    int rivalVelocity;
    int rivalHealth;

    float distanceToGoal;

    float turnAdvancePlayer;
    float turnAdvanceRival;
    float endTurnPositionPlayer;
    float endTurnPositionRival;
    float startTurnPositionPlayer;
    float startTurnPositionRival;

    public int speedCardCount;
    public int defenseCardCount;
    public int attackCardCount;

    public float timeRemaining;

    public enum AIAction { Velocidad, Defensa, Ataque }

    private System.Random random = new System.Random();

    [System.Serializable]
    public class Card{
        public int id_carta;
        public string nombre;
        public string tipo;
        public int costo;
        public int puntos_dano;
        public int puntos_defensa;
        public int puntos_velocidad;
        public string efecto;
        public string descripcion;
        
    }


    [System.Serializable]
    public class CardArray{
        public Card[] cards;
    }

    [System.Serializable]
    public class Car{
        public int id_coche;
        public string nombre;
        public int vida;
        public int velocidad;
        public int efecto_ataque;
        public int efecto_velocidad;
        public int efecto_defensa;
    }

    [System.Serializable]
    public class Scenario{
        public int id_scenario;
        public string nombre;
        public int distancia;
        public string descripcion;
        public int efecto_ataque;
        public int efecto_velocidad;
        public int efecto_defensa;
    }


    [System.Serializable]
    public class CarArray{
        public Car[] cars;
    }

    [System.Serializable]

    public class ScenarioArray{
        public Scenario[] scenarios;
    }


    private string apiUrlCars = "http://localhost:3000/api/cars";
    //public static CarsAPIManager Instance;
    public List<Car> Cars = new List<Car>();

    public List<Scenario> Scenarios = new List<Scenario>();



    // URL de tu endpoint de API
    private string apiUrlCards = "http://localhost:3000/api/cards";


    private string apiUrlScenarios = "http://localhost:3000/api/scenarios";
    // Lista para almacenar todas las cartas
    public List<Card> Cards = new List<Card>();


    //public static CardsAPIManager InstanceCards;

    [SerializeField] GameObject cardPrefab;

    [SerializeField] GameObject iconPrefab;
    [SerializeField] GameObject panel;

    [SerializeField] GameObject panelRival;

    [SerializeField] GameObject endBackground;

    List<GameObject> hand = new List<GameObject>();

    List<GameObject> icons = new List<GameObject>();

    List<Card> handAI = new List<Card>();
    List<int> deck = new List<int>();

    List<int> deckAI = new List<int>();

    List<int> discard = new List<int>();

    List<int> discardAI = new List<int>();
    private Dictionary<int, int> cardUsage = new Dictionary<int, int>();

    public TextMeshProUGUI timerText;

    public TextMeshProUGUI energyText;

    public TextMeshProUGUI endMessage;

    public TextMeshProUGUI rewardMessage;


void Start()
{
        //Connect to database using the API, and create the car sprites
        level = UnityEngine.Random.Range(1, 4);
        player_id = PlayerData.Instance.PlayerId;
        game_id = 1;
        coroutineFinished1 = false;
        coroutineFinished2 = false;
        coroutineFinished3 = false;
        coroutineFinished4 = false;
        checkStartGame = false;
        gameStarted = false;
        StartCoroutine(GetCardsCoroutine());
        StartCoroutine(GetCarsCoroutine());
        StartCoroutine(GetScenariosCoroutine());
        StartCoroutine(WaitForDeckToBeLoaded());
}

void Update()
{
    if (!gameStarted)
    {
        if (coroutineFinished1 && coroutineFinished2 && coroutineFinished3 && coroutineFinished4)
        {
            StartGame();
            gameStarted = true;
        }
    }
    if (coroutineFinished1 && coroutineFinished2 && checkStartGame && coroutineFinished3 && coroutineFinished4)
    {
        UpdateTimer();
        Move();
    }
}

void StartGame()
{
    // Assign starting values and initialize decks
    Scenario gameScenario = GetScenarioById(level);
    if (level == 1 || level == 2)
    {
        background.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Pista1");
    }
    else
    {
        background.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Pista2");
        background.GetComponent<Transform>().localScale = new Vector3(3, 2.7f, 2.7f);
    }
    background.GetComponent<Transform>().position = new Vector3(0, 0, 0);
    playerCar = Instantiate(carPrefab);
    playerCar.GetComponent<Transform>().position = new Vector3(-8, -.65f, 0);
    playerCar.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(MazoAPI.Instance.currentDeck.nombre_coche);
    rivalCar = Instantiate(carPrefab);
    rivalCar.GetComponent<Transform>().position = new Vector3(-8, .60f, 0);
    rivalCar.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("CocheVerde");
    InitDecks();
    Shuffle(deck);
    Shuffle(deckAI);
    turn++;
    energy = 10;
    timeRemaining = turnDuration; // Turn duration was reduced to 10 seconds for the prototype
    energyText.text = "Energy: " + Mathf.Floor(energy).ToString();
    player = GetCarById(1);
    rival = GetCarById(2);
    if (level != 1)
    {
        ApplyModifiers();
    }
    playerHealth = player.vida;
    rivalHealth = rival.vida;
    distanceToGoal = 16;
    playerHealthBar.GetComponent<Slider>().maxValue = player.vida;
    playerHealthBar.GetComponent<Slider>().minValue = 0;
    playerHealthBar.GetComponent<Slider>().value = playerHealth;
    rivalHealthBar.GetComponent<Slider>().maxValue = rival.vida;
    rivalHealthBar.GetComponent<Slider>().minValue = 0;
    rivalHealthBar.GetComponent<Slider>().value = rivalHealth;
    CreateHand();
    CreateHandAI();
    startTurnPositionPlayer = -8;
    startTurnPositionRival = -8;
    rival.velocidad += 70;
    endTurnPositionPlayer = DetermineNewPos(player.velocidad, playerCurrentPosition);
    endTurnPositionRival = DetermineNewPos(rival.velocidad, rivalCurrentPosition);
    playerVelocity = 1;
    rivalVelocity = 1;
    nextTurnPlayerVelocity = 1;
    nextTurnRivalVelocity = 1;
    CalcAdvance();
    checkStartGame = true;
}

void ApplyModifiers(){
    rival.vida += 50;
    rival.velocidad += 50;
    player.vida -= 30;
}
void InitDecks()
{
    int[] DifficultDeckAI = new int[] { 3, 4, 8, 16, 26, 7, 11, 1 };
    int[] NormalDeckAI = new int[] { 1, 4 ,2 ,13, 18, 26, 15, 29};
    for(int i = 0; i < MazoAPI.Instance.currentDeck.cartas.Count; i++)
    {
        deck.Add(MazoAPI.Instance.currentDeck.cartas[i].id_carta);
    }

    if (level == 1){
        for (int i = 0; i < NormalDeckAI.Length; i++){
            deckAI.Add(NormalDeckAI[i]);
        }
    }
    else{
        for (int i = 0; i < DifficultDeckAI.Length; i++){
            deckAI.Add(DifficultDeckAI[i]);
        }    
        }
    }

void Shuffle(List<int> deck)
{
    // Performs a random shuffle of the deck of cards.
    // It iterates over each element of the deck and swaps them randomly with another element.
    // The Random.Range() method from Unity is used to generate random indices.
    for (int i = 0; i < deck.Count; i++)
    {
        int temp = deck[i];
        int randomIndex = UnityEngine.Random.Range(i, deck.Count);
        deck[i] = deck[randomIndex];
        deck[randomIndex] = temp;
    }
}

public void RecordCardUsage(int card_id)
{
    // Check if the card is already in the dictionary
    if (cardUsage.ContainsKey(card_id))
    {
        // If the card is already present, increment its usage count
        cardUsage[card_id]++;
    }
    else
    {
        // If the card is not present, add it to the dictionary with usage count 1
        cardUsage.Add(card_id, 1);
    }
}

    void CreateHand()
    //Creates the player's hand each turn.
    {
        if (deck.Count == 0)
        //If there are no more cards to draw in the deck, the discard 
        //pile is moved to the deck and gets shuffled
            {
                deck.AddRange(discard);
                discard.Clear();
                Shuffle(deck);
            }
       while (hand.Count < 4)
       //Keeps adding cards until there are 4 in the player's hand each turn. 
        {
             if (deck.Count == 0)
             //If there are no more cards to draw in the deck, the discard 
            //pile is moved to the deck and gets shuffled
            {
                deck.AddRange(discard);
                discard.Clear();
                Shuffle(deck);
            }
            int id = deck[0]; //Assigns the top card's id from the deck each iteration
            deck.RemoveAt(0); //Removes the drawn card from the deck
            //Creates an instance of the card GameObject and assigns a listener
            GameObject newCard = Instantiate(cardPrefab, new Vector3( 2, -100, 0), Quaternion.identity);
            newCard.GetComponentInChildren<TextMeshProUGUI>().text = "";
            newCard.GetComponent<Image>().sprite = Resources.Load<Sprite>(id.ToString());
            newCard.GetComponent<Button>().onClick.AddListener(() => { CardSelect(id, newCard); });
            newCard.transform.SetParent(panel.transform);
            hand.Add(newCard); //The card is added to the hand
        }
    }

    void CreateHandAI()
    {
        if(deckAI.Count == 0){
        //If there are no more cards to draw in the deck, the discard 
        //pile is moved to the deck and gets shuffled
          deckAI.AddRange(discardAI);
          discardAI.Clear();
          Shuffle(deckAI);  
        }	
        while (handAI.Count < 4)
        //Keeps adding cards until there are 4 in the AI's hand each turn.
        {
             if (deckAI.Count == 0)
            {
                deckAI.AddRange(discardAI);
                discardAI.Clear();
                Shuffle(deckAI);
            }
            int id = deckAI[0]; //Assigns the top card's id from the deck each iteration
            deckAI.RemoveAt(0); //Removes the drawn card from the deck
            Card newCard = GetCardById(id);
            string tipo = newCard.tipo;
            handAI.Add(newCard);
            //Creates an instance of the icon game object to show the type of cards the AI has available during the turn
            GameObject iconCard = Instantiate(iconPrefab, new Vector3( 2, -100, 0), Quaternion.identity);
            iconCard.GetComponentInChildren<TextMeshProUGUI>().text = "";
            iconCard.GetComponent<Image>().sprite = Resources.Load<Sprite>(newCard.tipo);
            iconCard.transform.SetParent(panelRival.transform);
            iconCard.tag = tipo; //Assigns a tag to destroy the icon when the card is used
        }
    }


    void UpdateTimer()
    {
        //Updates timer and ends the turn when the time is over
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = "TIMER: " + Mathf.Floor(timeRemaining).ToString();
        }
        else
        {
            EndTurn();
        }
    }

     public float DetermineNewPos(int velocity, float currentPosition)
     //Determines the position on the screen where each car should appear according to the 
     //car's default velocity and the possible effects applied during the turn.
    {
        int xIncrement = velocity / turnDuration * 20;

        float newX = currentPosition + xIncrement;

        currentPosition =+ newX;
        return currentPosition;
    }

    void CalcAdvance(){
        turnAdvancePlayer = Math.Abs(endTurnPositionPlayer - startTurnPositionPlayer) / 100000 * playerVelocity * 0.5f;
        turnAdvanceRival = Math.Abs(endTurnPositionRival - startTurnPositionRival) / 100000 * rivalVelocity * 0.5f;
    }

    void Move(){
        playerCar.GetComponent<Transform>().position = new Vector3(playerCurrentPosition + turnAdvancePlayer, -.65f, 0f);
        playerCurrentPosition += turnAdvancePlayer;
        distanceToGoal = Math.Abs(playerCurrentPosition - 8);
        rivalCar.GetComponent<Transform>().position = new Vector3(rivalCurrentPosition + turnAdvanceRival, .60f, 0f);
        rivalCurrentPosition += turnAdvanceRival;
    }

    void CheckWin(){
        //Checks if the win conditions have been reached after each turn
        if(playerCurrentPosition > 8 || rivalHealth < 0){
            endBackground.GetComponent<SpriteRenderer>().sortingLayerName = "CarLayer";
            endBackground.GetComponent<SpriteRenderer>().sortingOrder = 10;
            endMessage.text = "VICTORY";
            rewardMessage.text = "REWARD";
            StartCoroutine(UpdateStats());
            GiveReward();
        }
        else if (rivalCurrentPosition > 8 || playerHealth < 0){
            endBackground.GetComponent<SpriteRenderer>().sortingLayerName = "CarLayer";
            endBackground.GetComponent<SpriteRenderer>().sortingOrder = 10;
            endMessage.text = "DEFEAT";
            StartCoroutine(SwitchToMenu());
        }
    }

    IEnumerator UpdateStats(){
        string url = "http://localhost:3000/api/games/" + game_id + "/stats";
        string json = WriteStatsToJson(player_id);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Stats Updated");
            }
        }
    }

    public string WriteStatsToJson(int winner_id)
    {
        // Construct the JSON string manually
        string json = "{";
        
        // Add winner_id
        json += "\"id_ganador\":" + winner_id + ",";
        
        // Add card_usage
        json += "\"uso_cartas\":[";
        foreach (KeyValuePair<int, int> entry in cardUsage)
        {
            json += "{";
            json += "\"id_carta\":" + entry.Key + ",";
            json += "\"numero_usos\":" + entry.Value;
            json += "},";
        }

        // Remove the trailing comma
        if (cardUsage.Count > 0)
        {
            json = json.Remove(json.Length - 1);
        }

        json += "],";
         // Add turn count
        json += "\"numero_turnos\":" + turn + "}";


        // Return the JSON string
        return json;
        
    }

    void EndTurn()
    {
        //Calls end of turn functions and resets energy, timer, and effects
        PlayAI();
        Move();
        UpdateHealthBars();
        CheckWin();
        timeRemaining = turnDuration;
        turn++;
        energy = 10;
        energyRival = 10;
        energyText.text = "Energy: "+ Mathf.Floor(energy).ToString();
        playerVelocity = nextTurnPlayerVelocity;
        rivalVelocity = nextTurnRivalVelocity;
        startTurnPositionPlayer = playerCurrentPosition;
        startTurnPositionRival = rivalCurrentPosition;
        endTurnPositionPlayer = DetermineNewPos(player.velocidad, startTurnPositionPlayer);
        endTurnPositionRival = DetermineNewPos(rival.velocidad, startTurnPositionRival);
        CalcAdvance();
        playerVelocity = 1;
        rivalVelocity = 1;
        nextTurnPlayerVelocity = 1;
        nextTurnRivalVelocity = 1;
        CreateHand();
        CreateHandAI();

    }

    void UpdateHealthBars(){
         playerHealthBar.GetComponent<Slider>().value = playerHealth;
         rivalHealthBar.GetComponent <Slider>().value = rivalHealth;
    }

    public void CardSelect(int id, GameObject card)
    {
        //Receives the id of a clicked card and applies its effects, then removes it from the hand
        //and the game scene
        Card currentCard = GetCardById(id);
        int cost = currentCard.costo;
        if (energy != 0 && energy - cost >= 0){
        energy -= cost;
        nextTurnPlayerVelocity += currentCard.puntos_velocidad;
        rivalHealth -= currentCard.puntos_dano;
        playerHealth += currentCard.puntos_defensa;
        energyText.text = "Energy: "+Mathf.Floor(energy).ToString();
        RecordCardUsage(id);
        discard.Add(id);
        hand.Remove(card);
        Destroy(card);
        UpdateHealthBars();
        CheckWin();
        }
    }

public string DetermineAIAction(int healthLevel, float distanceToGoal, int cardStrength, int energyLevel)
{
    // Determines AI actions for each turn *STILL UNDER DEVELOPMENT
    AIAction chosenAction; //=AIAction.Ataque; // Default action

    // Convierte los rangos numéricos en condiciones booleanas
    bool healthIsOk = healthLevel >= 1 && healthLevel <= 35;
    bool distanceIsOk = distanceToGoal >= 75 && distanceToGoal <= 99;
    bool cardStrengthIsOk = cardStrength >= 4 && cardStrength <= 5;
    bool energyLevelIsOk = energyLevel >= 7 && energyLevel <= 10;

    if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal >= 75 && distanceToGoal <= 99 && 
    cardStrength >= 4 && cardStrength <= 5 && energyLevel >= 7 && energyLevel <= 10)
    {
        chosenAction = AIAction.Velocidad;
    }
    else if (healthLevel > 35 && distanceToGoal >= 75 && distanceToGoal <= 99 && cardStrength >= 4 && 
    cardStrength <= 5 && energyLevel >= 7 && energyLevel <= 10)
    {
        chosenAction = AIAction.Velocidad;
    }
    else if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal < 75 && cardStrength >= 4 && 
    cardStrength <= 5 && energyLevel >= 7 && energyLevel <= 10)
    {
        chosenAction = AIAction.Defensa;
    }
    else if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal < 75 && cardStrength < 4 && 
    energyLevel >= 7 && energyLevel <= 10)
    {
        chosenAction = AIAction.Ataque;
    }
    else if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal >= 75 && distanceToGoal <= 99 && 
    cardStrength < 4 && energyLevel >= 7 && energyLevel <= 10)
    {
        chosenAction = AIAction.Velocidad;
    }
    else if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal < 75 && cardStrength < 4 && energyLevel < 7)
    {
        chosenAction = AIAction.Defensa;
    }
    else if (healthLevel > 35 && distanceToGoal < 75 && cardStrength < 4 && energyLevel >= 7 && energyLevel <= 10)
    {
        chosenAction = AIAction.Ataque;
    }
    else if (healthLevel > 35 && distanceToGoal >= 75 && distanceToGoal <= 99 && cardStrength >= 4 && 
    cardStrength <= 5 && energyLevel < 7)
    {
        chosenAction = AIAction.Velocidad;
    }
    else if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal >= 75 && distanceToGoal <= 99 && 
    cardStrength >= 4 && cardStrength <= 5 && energyLevel < 7)
    {
        chosenAction = AIAction.Defensa;
    }
    else if (healthLevel > 35 && distanceToGoal < 75 && cardStrength >= 4 && cardStrength <= 5 && energyLevel < 7)
    {
        chosenAction = AIAction.Ataque;
    }
    else if (healthLevel > 35 && distanceToGoal < 75 && cardStrength >= 4 && cardStrength <= 5 && 
    energyLevel >= 7 && energyLevel <= 10)
    {
        chosenAction = AIAction.Velocidad;
    }
    else if (healthLevel >= 1 && healthLevel <= 35 && distanceToGoal < 75 && cardStrength >= 4 && 
    cardStrength <= 5 && energyLevel < 7)
    {
        chosenAction = AIAction.Defensa;
    }
    else
    {
        // Por defecto, si ninguna condición se cumple
        chosenAction = AIAction.Ataque;
    }

    string finalAction = chosenAction.ToString();
    bool check = false;
    for (int i = 0; i < 4; i++)
    {
        string type = handAI[i].tipo;
        if (type == finalAction)
        {
            check = true;
        }
    }
    if (check == false)
    {
        return handAI[0].tipo;
    }
    else
    {
        return finalAction;
    }
}


    void PlayAI(){
        //Applies the effects of the card chosen by the AI.

        string tipoDeCarta = DetermineAIAction(rivalHealth, distanceToGoal, 100, energyRival);
        Card card = handAI.Find(card => card.tipo == tipoDeCarta.ToString());
        energyRival -= card.costo; 
        rivalVelocity = card.puntos_velocidad;
        playerHealth -= card.puntos_dano;
        rivalHealth += card.puntos_defensa; 
        discardAI.Add(card.id_carta);
        handAI.Remove(card);
        Destroy(GameObject.FindGameObjectWithTag(card.tipo));
}

void GiveReward(){
    if (MazoAPI.Instance != null && MazoAPI.Instance.currentDeck != null && MazoAPI.Instance.currentDeck.cartas != null)
        {
       HashSet<int> inventoryCardIds = InventoryAPI.Instance.GetInventoryCardIds();
            
            // Iterate through all available cards to find one not in the inventory
            reward_id = -1;
            for (int i = 0; i < Cards.Count; i++)
            {
                if (!inventoryCardIds.Contains(Cards[i].id_carta))
                {
                    reward_id = Cards[i].id_carta;
                    break;
                }
            }

            if (reward_id != -1)
            {
                StartCoroutine(Reward(reward_id));
            }
            else
            {
                Debug.Log("No new card available as a reward.");
            }
        }
        else
        {
            Debug.Log("MazoAPI or currentDeck is null.");
        }
    }



string WriteRewardToJson(int reward_id){
    string json = "{";
        
        // Add winner_id
        json += "\"id_carta\":" +  reward_id +",";
        
        // Add card_usage
        json += "\"id_coche\":" + player.id_coche + ",";
         // Add turn count
        json += "\"numero_cartas\":" + 1 + "}";

        // Return the JSON string
        return json;
}

IEnumerator Reward(int reward_id){
    string url = "http://localhost:3000/api/inventory/" + player_id + "/card";
    string json = WriteRewardToJson(reward_id);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Cards Updated");
            }
        }
        ShowReward(reward_id);
}

void ShowReward(int reward_id){
    foreach(var card in hand){
        Destroy(card);
    }
        GameObject newCard = Instantiate(cardPrefab, new Vector3( 2, -100, 0), Quaternion.identity);
        newCard.GetComponentInChildren<TextMeshProUGUI>().text = "";
        newCard.GetComponent<Image>().sprite = Resources.Load<Sprite>(reward_id.ToString());
        newCard.transform.SetParent(panel.transform);

    StartCoroutine(SwitchToMenu());
}

IEnumerator SwitchToMenu()
    {
    Destroy(playerCar);
    Destroy(rivalCar);
    yield return new WaitForSeconds(5.0f); 
    UnityEngine.SceneManagement.SceneManager.LoadScene("Menu_principal");
}

IEnumerator GetCardsCoroutine(){
    using (UnityWebRequest www = UnityWebRequest.Get(apiUrlCards)){
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success){
            Debug.LogError("Error al obtener las cartas: " + www.error);
        }
        else{
            ProcessCardsResponse(www.downloadHandler.text);
        }
    }
    coroutineFinished1 = true;
}

    IEnumerator GetCarsCoroutine(){
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrlCars)){
            yield return www.SendWebRequest();

            if(www.result != UnityWebRequest.Result.Success){
                Debug.LogError("Error al obtener los coches: " + www.error);
            }
            else{
                ProcessCarsResponse(www.downloadHandler.text);
            }
        }
        coroutineFinished2 = true;
    }

    IEnumerator GetScenariosCoroutine(){
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrlScenarios)){
            yield return www.SendWebRequest();

            if(www.result != UnityWebRequest.Result.Success){
                Debug.LogError("Error al obtener los coches: " + www.error);
            }
            else{
                ProcessScenariosResponse(www.downloadHandler.text);
            }
        }
        coroutineFinished3 = true;
    }

    public void ProcessCardsResponse(string jsonResponse){
        string adjustedJson = "{\"cards\":" + jsonResponse + "}";
        CardArray cardArray = JsonUtility.FromJson<CardArray>(adjustedJson);
        Cards.Clear(); 
        Cards = new List<Card>();
        foreach(var card in cardArray.cards){
            Cards.Add(card);
            Debug.Log("Card Name: " + card.nombre);
        }
    }   

    // Procesar la respuesta de la API y almacenar las cartas
    public void ProcessScenariosResponse(string jsonResponse){
        string adjustedJson = "{\"scenarios\":" + jsonResponse + "}";
        ScenarioArray scenarioArray = JsonUtility.FromJson<ScenarioArray>(adjustedJson);
        Scenarios.Clear(); 
        Scenarios = new List<Scenario>();
        foreach(var scenario in scenarioArray.scenarios){
            Scenarios.Add(scenario);
            Debug.Log("Scenario Name: " + scenario.nombre);
        }
    }

     public void ProcessCarsResponse(string jsonResponse){
        string adjustedJson = "{\"cars\":" + jsonResponse + "}";
        CarArray carArray = JsonUtility.FromJson<CarArray>(adjustedJson);

        Cars.Clear();
        Cars = new List<Car>();

        foreach (Car car in carArray.cars){
            Cars.Add(car);
            Debug.Log("ID Coche: " + car.id_coche + ", Nombre: " + car.nombre + ", Vida: " + car.vida + ", Velocidad: " + car.velocidad + ", Efecto Ataque: " + car.efecto_ataque + ", Efecto Velocidad: " + car.efecto_velocidad + ", Efecto Defensa: " + car.efecto_defensa);
        }
    }

     private IEnumerator WaitForDeckToBeLoaded()
    {
        // Esperamos que MazoAPI se inicialice y tenga un mazo cargado
        yield return new WaitUntil(() => MazoAPI.Instance != null && MazoAPI.Instance.currentDeck != null);

        coroutineFinished4 = true;
    }

    // Método para obtener una card por su ID
    public Card GetCardById(int id){
        return Cards.Find(card => card.id_carta == id);
    }

    public Car GetCarById(int id){
        return Cars.Find(car => car.id_coche == id);
    }

    public Scenario GetScenarioById(int id){
        return Scenarios.Find(scenario => scenario.id_scenario == id);
    }
}