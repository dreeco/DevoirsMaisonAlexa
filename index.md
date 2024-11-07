---
_layout: landing
---

# Welcome to the documentation of the "Devoirs Maison" skill code

## CleanArchi

This code is based on the "[CleanArchi](https://dev.to/julianlasso/clean-architecture-domain-layer-3bdd)" pattern.
A breakdown of the code into different layers:
- Presentation: The layer representing the final application. It is aware of all other layers, especially Infrastructure and Application, which it links together.
- Application: The layer representing application-specific code (routing, text, etc.). It uses the domain to structure it for the application.
- Domain: The core business logic, representing interactions, entities, etc.
- Infrastructure: The layer implementing external services (data, APIs, etc.)

In this skill, here is the code distribution:
- Presentation: The skill's handler. It has minimal logic, parses the request, and forms the response.
- Application: Routes based on the session state, retrieves the next Intent.
- Domain: The core of the exercises and the question/answer engine.
- Infrastructure: Code related to Alexa integration.

## Here’s the flow of a user request

### Presentation
---

- The Presentation layer receives the request. See [FunctionHandler](xref:DevoirsAlexa.Presentation.Function).
- It transfers user session information to [SessionData](xref:DevoirsAlexa.Infrastructure.Models.HomeworkSession), which is an implementation within the Infrastructure layer of the [interface](xref:DevoirsAlexa.Domain.Models.IHomeworkSession) used by the domain to track the session's state.
- Then, it calls the Application layer to convert the request into an Alexa response using the [RequestsHandler](xref:DevoirsAlexa.Application.Handlers.RequestsHandler).

### Application
---

- The requestHandler retrieves the current [HomeworkStep](xref:DevoirsAlexa.Application.Enums.HomeworkStep) and uses it to determine the response.
- It fills the prompt and reprompt by using the [SentenceBuilder](xref:DevoirsAlexa.Infrastructure.SentenceBuilder), an Infrastructure layer implementation of the domain's [ISentenceBuilder](xref:DevoirsAlexa.Domain.ISentenceBuilder). It may, for example, ask for the user’s first name or class.
- If an exercise is in progress, the [ExercisesRunner](xref:DevoirsAlexa.Domain.HomeworkExercisesRunner.ExerciceRunner) from the domain takes over to create an [AnswerResult](xref:DevoirsAlexa.Domain.Models.AnswerResult), containing answer validation as well as the next question.

### Domain - During an Exercise Only
---

- The runner instantiates the correct [exercise](xref:DevoirsAlexa.Domain.HomeworkExercises.IExerciceQuestionsRunner) using the [dispatcher](xref:DevoirsAlexa.Domain.HomeworkExercisesRunner.ExerciceDispatcher).
- To see all exercise types, refer to the [exercise documentation](xref:DevoirsAlexa.Domain.Enums.HomeworkExercisesTypes).
- User answers are validated, and the next question is generated via [ValidateAnswerAndGetNext](xref:DevoirsAlexa.Domain.HomeworkExercisesRunner.ExerciceRunner).
- This method calls validation and retrieves the question based on the specific exercise. Each exercise has its own question generator and answer validation.
- An [AnswerResult](xref:DevoirsAlexa.Domain.Models.AnswerResult) is returned with:
  - [AnswerValidation](xref:DevoirsAlexa.Domain.Models.AnswerValidation): Indicates whether the answer is correct and provides the correct answer if needed.
  - [Question](xref:DevoirsAlexa.Domain.Models.Question): Contains the next question to ask the user. Empty if the exercise is complete.
  - [ExerciceResult](xref:DevoirsAlexa.Domain.Models.ExerciceResult): Summarizes the exercise, including score and duration. Empty if the exercise is incomplete.
  - [HelpResult](xref:DevoirsAlexa.Domain.Models.HelpResult): Provides help for answering the question. Filled if the user requested help.

### Application
---

- The Application layer receives information from the exercise and either presents a new question or summarizes the exercise to the user.
- It also passes the next expected intent to the Presentation layer via [NextRequestRouting](xref:DevoirsAlexa.Application.Handlers.NextRequestRouting) using the Intents configuration table.

### Presentation
---

- The Presentation layer converts the prompt and reprompt into an Alexa response using the SentenceBuilder.
- It also includes the next expected Intent in the response.
