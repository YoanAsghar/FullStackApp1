const LOGIN_FORM = document.getElementById("login-form");
const REGISTER_FORM = document.getElementById("register-form");
const LOGOUT_BTN = document.getElementById("logout-btn");

LOGIN_FORM.addEventListener("submit", (event) => {
  event.preventDefault();
  LoginUser();
});
REGISTER_FORM.addEventListener("submit", (event) => {
  event.preventDefault();
  RegisterUser();
});

LOGOUT_BTN.addEventListener("click", () => {
    localStorage.removeItem("jwt_token");
    location.reload();
});

async function checkIfUserIsAuthorized(){
  try{
    const jwt_token = localStorage.getItem("jwt_token");

    if(jwt_token){
      await fetch("/api/products/", {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${jwt_token}`
        }
      }).then(response => {
        if(response.ok){
          console.log(response);
          document.querySelector(".forms-container").classList.add("hidden");
          document.getElementById("content").classList.remove("hidden");
        }
      });
    }
  }catch(err){
    alert(err);
    return `There was an error ${err}`
  }
}
checkIfUserIsAuthorized();

function RegisterUser(){
  try {
    fetch("/api/auth/register", {
      method: "POST",
      headers: {
        'Content-Type': 'application/json',
      },
      body: GetDataFromUserRegister()

    }).then(response => {
        if(!response.ok){
          throw new Error('Network NOT OK!');
        }
        console.log(response)
        return response.json();
      });
  } catch (err) {
    throw err("There was an error", err)
  }
}

async function LoginUser(){
  try{
    const response = await fetch("/api/auth/login", {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: GetDataFromUserLogin()
    })
    

    if(!response.ok){
      throw new Error("damn man, an error");
    }

    const data = await response.json();
    const token = data.token;

    if(token){
      localStorage.setItem("jwt_token", token)
      console.log("Jwt saved correctly");

      document.querySelector(".forms-container").classList.add('hidden');
      document.getElementById("content").classList.remove('hidden');
    }
  } catch (err){
    return `There was an error`, err
  }
}


function GetDataFromUserRegister(){
  const username_input = document.getElementById("register-username").value;
  const email_input = document.getElementById("register-email").value;
  const password_input = document.getElementById("register-password").value;

  return JSON.stringify({
    Username: username_input,
    Email: email_input,
    passwordHash: password_input
  });
}

function GetDataFromUserLogin(){
  const email_input = document.getElementById("login-email").value;
  const password_input = document.getElementById("login-password").value;

  return JSON.stringify({
    Email: email_input,
    passwordHash: password_input
  });
}
