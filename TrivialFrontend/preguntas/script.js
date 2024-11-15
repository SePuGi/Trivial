// Variable para almacenar las preguntas obtenidas desde la API
var questions = [];

// Variable para el puntaje
var score = 0;

// Variable para la categoría seleccionada
var selectedCategory = null;

// URL de la API para actualizar la puntuación
const gameApiUrl = 'https://26.39.250.148:7230/api/Game/SaveGame';

// Obtiene las preguntas desde la API
document.addEventListener('DOMContentLoaded', function () {

    // Función para obtener los datos de la API de trivia
    function fetchTriviaData(categoria) {
        // Guardamos la categoría seleccionada
        selectedCategory = categoria;

        // URL de la API de trivia
        const apiUrl = 'https://opentdb.com/api.php?amount=3&category=' + categoria;
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

    function fetchCategories() {
        const url = 'https://26.39.250.148:7230/api/Game/category';

        // Crear una solicitud AJAX con Fetch API
        fetch(url)
            .then(response => {
                // Verificamos si la respuesta es exitosa
                if (!response.ok) {
                    throw new Error('Error al obtener datos');
                }
                return response.json();
            })
            .then(data => {
                // Llenamos el select con las opciones obtenidas de la API
                const select = document.getElementById('categorySelect');
                data.forEach(category => {
                    const option = document.createElement('option');
                    option.value = category.id;  // Supongo que el id es el valor que deseas
                    option.textContent = category.name;  // El nombre de la categoría es el texto visible
                    select.appendChild(option);
                });
            })
            .catch(error => {
                console.error('Error en la solicitud:', error);
            });
    }

    // Función que se ejecuta cuando se selecciona una categoría
    function onCategorySelect() {
        const select = document.getElementById('categorySelect');
        const selectedValue = select.value;
        
        if (selectedValue) {
            console.log('Categoría seleccionada:', selectedValue);
            fetchTriviaData(selectedValue);
            // Aquí puedes realizar la lógica que desees con el valor seleccionado
        } else {
            console.log('No se ha seleccionado una categoría');
        }
    }

    // Llamamos a la función para obtener las categorías cuando se carga la página
    window.onload = function() {
        fetchCategories();

        // Escuchamos el evento 'change' del select para ejecutar la función
        const select = document.getElementById('categorySelect');
        select.addEventListener('change', onCategorySelect);
    };
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
        
        // Enviar la puntuación final al terminar el juego
        updateGameAPI();
        
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
                    
                    // Enviar la puntuación final al terminar el juego
                    updateGameAPI();
                    return;
                }

                // Si la respuesta es correcta y hay más preguntas, pasar a la siguiente pregunta
                setTimeout(function () {
                    createQuestion();
                }, 500);

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

                return;
            }
        }
    }
}

function updateGameAPI() {
    const token = localStorage.getItem('sessionToken');

    // Verificar si el token está presente
    if (!token) {
        console.error('Token no encontrado. No se puede actualizar la puntuación.');
        return;  // No se envía la solicitud si no hay token
    }

    // Verificar los valores de `selectedCategory` y `score` antes de enviarlos
    console.log('Categoria seleccionada:', selectedCategory);
    console.log('Puntuación:', score);

    // Asegúrate de que los valores sean números válidos
    const categoryId = parseInt(selectedCategory);
    const correctAnswers = parseInt(score);

    // Verificar si los valores son válidos
    if (isNaN(categoryId) || isNaN(correctAnswers)) {
        console.error('Los valores de categoryId o correctAnswers no son válidos.');
        return;  // No enviar los datos si alguno es inválido
    }

    // Crear el objeto `data` a enviar
    const data = {
        categoryId: categoryId,  // ID de la categoría
        correctAnswers: correctAnswers  // Número de respuestas correctas
    };

    console.log('Enviando datos a la API:', data);

    // Realizar la solicitud HTTP con Fetch
    fetch(gameApiUrl, {
        method: 'POST',  // Utilizar POST para enviar los datos
        headers: {
            'Content-Type': 'application/json',  // Indicar que estamos enviando JSON
            'Authorization': `Bearer ${token}`  // Incluir el token de autorización
        },
        body: JSON.stringify(data)  // Convertir los datos a JSON
    })
    .then(response => {
        console.log('Estado de la respuesta:', response.status);  // Verificar el código de estado de la respuesta
        if (!response.ok) {
            throw new Error('Error al actualizar la puntuación: ' + response.statusText);  // Lanzar un error si la respuesta no es exitosa
        }
        return response;  // Intentar convertir la respuesta a JSON
    })
    .then(responseData => {
        if (responseData) {
            console.log('Respuesta de la API:', responseData);
            // Aquí puedes procesar la respuesta de la API, si es necesario
        } else {
            console.log('No se recibió respuesta en formato JSON');
        }
    })
    .catch(error => {
        console.error('Hubo un problema con la solicitud de actualización:', error);
    });
}
