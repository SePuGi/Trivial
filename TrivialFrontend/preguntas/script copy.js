// Variable para almacenar las preguntas obtenidas desde la API
var questions = [];

// Variable para el puntaje
var score = 0;

// URL de la API para actualizar la puntuación
const gameApiUrl = 'https://26.39.250.148:7230/api/Game/SaveGame';

// Obtiene las preguntas desde la API
document.addEventListener('DOMContentLoaded', function () {
    // URL de la API de trivia
    const apiUrl = 'https://opentdb.com/api.php?amount=10&type=multiple';
    

    // Función para obtener los datos de la API de trivia
    function fetchTriviaData() {
        fetch(apiUrl)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Error de red ' + response.statusText);
                }
                return response.json(); // Convertir la respuesta a JSON
            })
            .then(data => {
                // Mapear los datos de la API a la estructura que usamos
                questions = data.results.map(item => ({
                    question: item.question,
                    correct: item.correct_answer,
                    answers: [...item.incorrect_answers, item.correct_answer] // Incluimos las respuestas incorrectas y la correcta
                }));

                // Iniciar el juego después de cargar las preguntas
                startGame();
            })
            .catch(error => {
                console.error('Hubo un problema con la solicitud fetch:', error);
            });
    }

    // Llamar a la función para obtener los datos de la trivia
    fetchTriviaData();
});

// Variable para hacer referencia al formulario de preguntas en el HTML
var questionForm = document.getElementById("questionForm");

// Función para iniciar el juego
function startGame() {
    // Limpiar cualquier HTML anterior
    questionForm.innerHTML = "";

    // Establecer el estilo del formulario para centrar los elementos
    questionForm.style.margin = "15% auto";

    // Cargar la primera pregunta
    createQuestion();
}

// Función para mezclar las respuestas aleatoriamente (Fisher-Yates)
function shuffleAnswers(answers) {
    for (let i = answers.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [answers[i], answers[j]] = [answers[j], answers[i]]; // Intercambiar los elementos
    }
    return answers;
}

// Función para crear una pregunta y sus respuestas
function createQuestion() {
    // Limpiar cualquier HTML anterior
    questionForm.innerHTML = "";

    // Comprobar si hay preguntas disponibles
    if (questions.length === 0) {
        // Si no hay preguntas, finalizar el juego
        questionForm.innerHTML = "<h1>¡Juego terminado! Has respondido todas las preguntas. Puntuación final: " + score + " puntos.</h1>";
        return;
    }

    // Tomar la primera pregunta de la lista
    var currentQuestion = questions[0];

    // Mezclar las respuestas aleatoriamente
    var shuffledAnswers = shuffleAnswers([...currentQuestion.answers]); // Usamos una copia para no modificar el array original

    // Crear los elementos del formulario para la pregunta
    var formGroup = document.createElement("div");
    var questionEl = document.createElement("h2");

    // Asignar clase al formulario y al título de la pregunta
    formGroup.className = "formGroup";
    questionEl.id = "questions";

    // Crear el texto de la pregunta
    var questionText = document.createTextNode(currentQuestion.question);

    // Agregar el texto de la pregunta al elemento
    questionEl.appendChild(questionText);

    // Agregar el título de la pregunta al contenedor del formulario
    formGroup.appendChild(questionEl);
    questionForm.appendChild(formGroup);

    // Agregar las respuestas al formulario
    for (var j = 0; j < shuffledAnswers.length; j++) {
        var answerDiv = document.createElement("div");
        var answerEl = document.createElement("input");

        // Crear el texto de la respuesta
        var answerText = document.createTextNode(shuffledAnswers[j]);

        // Agregar la respuesta al div
        answerDiv.appendChild(answerEl);
        answerDiv.appendChild(answerText);

        // Asignar clases y atributos al input
        answerDiv.className = "questionWrap";
        answerEl.type = "radio";
        answerEl.name = "radio";
        answerEl.value = shuffledAnswers[j];

        // Agregar la respuesta al formulario
        formGroup.appendChild(answerDiv);
    }

    // Crear el botón de enviar
    var submitBtn = document.createElement("button");
    submitBtn.className = "btn btn-lg btn-primary";
    submitBtn.textContent = "Enviar Respuesta";
    submitBtn.type = "button";
    submitBtn.onclick = submitAnswer;

    // Agregar el botón de enviar al formulario
    questionForm.appendChild(submitBtn);
}

// Función para enviar la respuesta
function submitAnswer() {
    // Obtener todos los inputs de respuestas
    var els = document.getElementsByTagName("input");

    // Recorrer los inputs para ver cuál está seleccionado
    for (var i = 0; i < els.length; i++) {
        if (els[i].checked) {
            if (els[i].value.trim() == questions[0].correct.trim()) {
                // Si la respuesta es correcta, sumar un punto
                console.log("Respuesta Correcta", els[i]);
                score++;  // Incrementar el puntaje

                // Cambiar el color de la respuesta correcta
                els[i].parentElement.className = "questionWrap right";

                // Eliminar la primera pregunta del array
                questions.shift();

                // Verificar si hay más preguntas, si no, finalizar el juego
                if (questions.length == 0) {
                    questionForm.innerHTML = "<h1>¡Juego terminado! Has respondido todas las preguntas correctamente. Puntuación final: " + score + " puntos.</h1>";
                    return;
                }

                // Si la respuesta es correcta y hay más preguntas, pasar a la siguiente pregunta
                setTimeout(function () {
                    createQuestion();
                }, 500);

                // Enviar la puntuación a la API de actualización
                updateGameAPI();

                return;
            } else {
                // Si la respuesta es incorrecta, mostrar mensaje en consola y pasar a la siguiente pregunta
                console.log("Respuesta Incorrecta");

                // Resaltar la respuesta incorrecta
                for (var j = 0; j < els.length; j++) {
                    if (els[j].checked) {
                        els[j].parentElement.className = "questionWrap wrong";
                    }
                }

                // Eliminar la primera pregunta del array sin sumar puntos
                questions.shift();

                // Pasar a la siguiente pregunta
                setTimeout(function () {
                    createQuestion();
                }, 500);

                // Enviar la puntuación a la API de actualización
                updateGameAPI();

                return;
            }
        }
    }
}

// Función para actualizar la puntuación y el número de partida en la API
function updateGameAPI() {
    // Datos a enviar a la API (ajusta según sea necesario)
    const data = {
        score: score, // La puntuación actual
        gameNumber: 1 // Incrementar el número de la partida (puedes cambiar esta lógica según tus necesidades)
    };

    // Enviar los datos a la API usando Fetch
    fetch(gameApiUrl, {
        method: 'POST', // O 'PATCH' si ya existe un recurso
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Error al actualizar la puntuación: ' + response.statusText);
        }
        console.log("Puntuación y número de partida actualizados correctamente.");
    })
    .catch(error => {
        console.error('Hubo un problema con la solicitud de actualización:', error);
    });
}
