// Esperamos que el DOM esté completamente cargado antes de ejecutar el script
document.addEventListener('DOMContentLoaded', function () {
    // Función para eliminar los espacios en tiempo real en un campo de entrada
    function preventSpaces(event) {
        // Reemplazar todos los espacios en blanco por una cadena vacía
        event.target.value = event.target.value.replace(/\s+/g, '');
    }

    // Obtener los campos de entrada
    const inputs = document.querySelectorAll('input[type="text"]');
    const inputsPass = document.querySelectorAll('input[type="password"]');

    // Añadir el evento para prevenir los espacios en los campos de texto
    inputs.forEach(input => {
        input.addEventListener('input', preventSpaces);
    });
    inputsPass.forEach(input => {
        input.addEventListener('input', preventSpaces);
    });

    // Obtenemos los elementos necesarios
    const loginTab = document.querySelector('button.active');
    const registerTab = document.querySelector('button.inactive');
    const loginDiv = document.getElementById('login');
    const registerDiv = document.getElementById('registro');
    const checkbox = document.getElementById('showpass');
    const showpassT = document.getElementById('showpassT');

    // Función para mostrar el login y ocultar el registro
    function showLogin() {
        loginDiv.style.display = 'block';
        registerDiv.style.display = 'none';
        loginTab.classList.add('active');
        loginTab.classList.remove('inactive');
        registerTab.classList.remove('active');
        registerTab.classList.add('inactive');
    }

    // Función para mostrar el registro y ocultar el login
    function showRegister() {
        registerDiv.style.display = 'block';
        loginDiv.style.display = 'none';
        registerTab.classList.add('active');
        registerTab.classList.remove('inactive');
        loginTab.classList.remove('active');
        loginTab.classList.add('inactive');
    }
    // Cambiar tipo de los inputs de contraseña
    checkbox.addEventListener('change', function () {
        // Cambiar todos los campos de contraseña
        inputsPass.forEach(input => {
            if (checkbox.checked) {
                input.type = "text"; // Mostrar contraseña
            } else {
                input.type = "password"; // Ocultar contraseña
            }
        });
    });
    // Llamada para manejar el Login con AJAX
    function LoginForm(event) {
        event.preventDefault(); // Evitar el envío tradicional del formulario
    
        const userL = document.getElementById('userL').value;
        const passL = document.getElementById('passL').value;
    
        const data = {
            name: userL,
            password: passL
        };
    
        const xhr = new XMLHttpRequest();
        xhr.open('POST', 'https://26.39.250.148:7230/api/User/login', true);
        xhr.setRequestHeader('Content-Type', 'application/json');
    
        // Manejo de la respuesta de la solicitud
        xhr.onload = function () {
            if (xhr.status >= 200 && xhr.status < 300) {
                try {
                    const response = JSON.parse(xhr.responseText); // Convertir la respuesta a JSON
                    const token = response.token; // Asegúrate de que "token" sea la clave correcta en la respuesta
    
                    if (token) {
                        localStorage.setItem("sessionToken", token); // Guarda el token en localStorage
                        console.log('Login exitoso! Token guardado:', token);
                        alert('Login exitoso!');
                        window.location.href = './principal/index.html'; // Cambia esta URL a donde quieras redirigir
                    } else {
                        console.error('Error: Token no encontrado en la respuesta.');
                        alert('Error al iniciar sesión. Inténtalo de nuevo.');
                    }
                } catch (error) {
                    console.error('Error al procesar la respuesta del servidor:', error);
                    alert('Error inesperado. Inténtalo de nuevo.');
                }
            } else {
                console.error('Usuario o contraseña incorrectos:', xhr.responseText);
                alert('Usuario o contraseña incorrectos');
            }
        };
    
        xhr.onerror = function () {
            console.error("Hubo un error con la solicitud AJAX.");
            alert("Hubo un error al intentar conectarse con la API.");
        };
    
        // Enviar los datos como JSON
        xhr.send(JSON.stringify(data));
    }
    

    // Llamada para manejar el Registro con AJAX
    function RegisterForm(event) {
        event.preventDefault(); // Evitar el envío tradicional del formulario

        const userR = document.getElementById('userR').value;
        const passR = document.getElementById('passR').value;
        const passC = document.getElementById('passC').value;

        if (passR !== passC) {
            alert("Las contraseñas no coinciden.");
            return;
        }

        const data = {
            name: userR,
            password: passR
        };

        const xhr = new XMLHttpRequest();
        xhr.open('POST', 'https://26.39.250.148:7230/api/User/register', true);
        xhr.setRequestHeader('Content-Type', 'application/json');

        // Enviar los datos como JSON
        xhr.send(JSON.stringify(data));

        // Manejo de la respuesta de la solicitud
        xhr.onload = function () {
            if (xhr.status >= 200 && xhr.status < 300) {
                console.log('Registro exitoso!', xhr.responseText);
                alert('Registro exitoso!');
                showLogin(); // Mostrar login después del registro
            } else {
                console.error('Error al registrar el usuario', xhr.responseText);
                alert('Error al registrar el usuario');
            }
        };

        xhr.onerror = function () {
            console.error("Hubo un error con la solicitud AJAX.");
            alert("Hubo un error al intentar conectarse con la API.");
        };
    }

    // Asociar las funciones a los formularios
    const loginForm = document.getElementById('loginForm');
    loginForm.addEventListener('submit', LoginForm);

    const registerForm = document.getElementById('registerForm');
    registerForm.addEventListener('submit', RegisterForm);

    // Añadimos los eventos de clic a los encabezados para cambiar entre login y registro
    loginTab.addEventListener('click', showLogin);
    registerTab.addEventListener('click', showRegister);


    // Añadimos los eventos de clic a los encabezados
    loginTab.addEventListener('click', showLogin);
    registerTab.addEventListener('click', showRegister);

    // Inicializamos el estado por defecto (Login visible)
    showLogin();
});