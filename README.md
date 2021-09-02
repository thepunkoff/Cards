# Cards (alpha)

A simple solution for language learners for reviewing flashcards with no need of creating them manually. Just give it a word and it will return the translation and the detailed summary for the word including use cases, etymology, pronunciation and more. You can then chose to add the flashcard to your deck or not.

## Features:
- Create flashcards with detailed word info by just typing the word in target language
- Chose to save cards to review them afterwards, or just use it as a translator with a detailed output.
- Feel the power of spaced repetition method and learn loads of new words in no time!
- Create users in the system and separate their decks. Deploy a little Cards server for your family, company, you geek friends, a community of language learning enthusiasts or just an instance for yourself.

**Currently supports:**
- MongoDb as a data storage
- SM-2 (SuperMemo 2) spaced repetition algorithm
- RU -> EN translation via IBM Cloud Translator.
- Telegram Bot client.

## Future plans:
- Learning summary
- Choice of native and target language
- Mobile app
- Client-side caching
- More algorithms
- Some more API tweaks, code cleanup and refactoring, and a lot more.

## Try it out!
You have to prepare 3 config files and put them into 'Configuration' folder in your workspace directory.
- **domain.json**
```
{
  "IbmCloudToken": "<you ibm cloud token>",
  "MongoConnectionString": "mongodb://localhost:27017",
  "MongoDatabaseName": "cards"
}
```
- **server.json**
```
{
  "Host": "localhost", 
  "Port": 25204
}
```
- **bot.json**
```
{
  "Token": "<your telegram bot token>",
  "CardsAddress": "http://localhost:25204"
}
```
Then just start the Cards server, the Telegram Bot client and MongoDb. You can do it all at once via docker-compose:  
``docker-compose -f docker-compose.yml up -d``  
(You can get the sample yml file [here](docker-compose.yml))

Now you can send commands to your Telegram bot to communicate with the server.

## Telegram Bot commands

### \<word\>
Just send a word to the Bot, and it will give you the detailed info for the word.

### /login \<username\> \<password\>
Logs in into the system or registers and logs in automatically. Now you can learn words. Send any word again to see a "Learn" button under the summary. Press the button to save it to your deck. Press it again to "forget" the word and delete it from your deck. It will no longer available for review.

### /review
Initiates a review session. Bot sends you a word in target language. Press the 'Flip' button and then estimate how good you were at recalling the word meaning. Keep sending '/review' again until there is no more words to review today. Check the bot every day to try and review new cards. The algorithm tries to adapt to your answers and schedule words intelligently to make learning the most efficient.