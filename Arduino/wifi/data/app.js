let wifis = [];

async function getWifiAvailableList() {
	const resp = await fetch("/scan");
	const data = await resp.json();
	console.log(data);
	wifis = data.networks;
	let wifi_list = generateWifiHtml();
	document.querySelector(".wifi-list").innerHTML = wifi_list;
}
function generateWifiHtml() {
	let s = "<table><tr><th>SSID</th></tr>";
	s += wifis
		.map((x) => {
			return `<tr><td onclick="wifiConnect(event)">${x.ssid}</td></tr>`;
		})
		.join("");
	s += "</table>";

	return s;
}

function wifiConnect(event) {
	const ssid = event.target.innerHTML;
	const connectModal = document.createElement("div");
	connectModal.setAttribute("class", "connect-modal");
	let closeButton = document.createElement("button");
	closeButton.innerHTML = "X";
	closeButton.setAttribute("id", "close-button");
	closeButton.onclick = function () {
		connectModal.remove();
	};
	let ssidInput = document.createElement("input");
	ssidInput.setAttribute("type", "text");
	ssidInput.value = ssid;
	ssidInput.setAttribute("id", "ssid-input");
	ssidInput.setAttribute("readonly", true);
	let passwordInput = document.createElement("input");
	passwordInput.setAttribute("type", "password");
	passwordInput.setAttribute("placeholder", "Jelszó");
	passwordInput.setAttribute("id", "password");
	passwordInput.value = "20190918JV";
	let connectButton = document.createElement("button");
	connectButton.innerHTML = "Csatlakozás";
	connectButton.onclick = function () {
		let password = document.getElementById("password").value;
		SendWifiConnection(ssid, password);
	};
	connectModal.appendChild(ssidInput);
	connectModal.appendChild(document.createElement("br"));
	connectModal.appendChild(passwordInput);
	connectModal.appendChild(document.createElement("br"));
	connectModal.appendChild(closeButton);
	connectModal.appendChild(connectButton);
	document.body.appendChild(connectModal);
}

async function SendWifiConnection(ssid, password) {
	let data = { ssid: ssid, password: password };
	let jsonData = JSON.stringify(data);
	try {
		const resp = await fetch("/connect", {
			method: "POST",
			headers: {
				"Content-Type": "application/json"
			},
			body: jsonData
		});
		const result = await resp.text();
		const modal = document.querySelector(".connect-modal");
		modal.innerHTML = result;
		createLoginHtml();
	} catch (error) {
		const modal = document.querySelector(".connect-modal");
		modal.innerHTML = error;
	}
}

function createLoginHtml() {
	const connectModal = document.createElement("div");
	connectModal.setAttribute("class", "connect-modal");
	let closeButton = document.createElement("button");
	closeButton.innerHTML = "X";
	closeButton.setAttribute("id", "close-button");
	closeButton.onclick = function () {
		connectModal.remove();
	};
	let emailInput = document.createElement("input");
	emailInput.setAttribute("type", "email");
	emailInput.setAttribute("id", "email-input");
	emailInput.setAttribute("placeholder", "E-mail cím");
	emailInput.value = "admin@gmail.com";
	let passwordInput = document.createElement("input");
	passwordInput.setAttribute("type", "password");
	passwordInput.setAttribute("placeholder", "Jelszó");
	passwordInput.setAttribute("id", "password");
	passwordInput.value = "asd123";
	let connectButton = document.createElement("button");
	connectButton.innerHTML = "Bejelentkezés";
	connectButton.onclick = function () {
		let email = document.getElementById("email-input").value;
		let password = document.getElementById("password").value;
		SendLoginData(email, password);
	};
	connectModal.appendChild(emailInput);
	connectModal.appendChild(document.createElement("br"));
	connectModal.appendChild(passwordInput);
	connectModal.appendChild(document.createElement("br"));
	connectModal.appendChild(closeButton);
	connectModal.appendChild(connectButton);
	document.body.appendChild(connectModal);
}

async function SendLoginData(email, password) {
	let data = { email: email, password: password };
	let jsonData = JSON.stringify(data);
	try {
		const resp = await fetch("/login", {
			method: "POST",
			headers: {
				"Content-Type": "application/json"
			},
			body: jsonData
		});
		const result = await resp.text();
		const modal = document.querySelector(".connect-modal");
		alert(result);
		console.log(result);
	} catch (error) {
		const modal = document.querySelector(".connect-modal");
		alert(result);
		console.log(result);
	}
}
