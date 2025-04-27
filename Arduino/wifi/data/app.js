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
	let connectButton = document.createElement("button");
	connectButton.innerHTML = "Csatlakozás";
	connectButton.onclick = function () {
		let password = document.getElementById("password").value;
		console.log(`SSID: ${ssid}, Password: ${password}`);
	};
	connectModal.appendChild(ssidInput);
	connectModal.appendChild(document.createElement("br"));
	connectModal.appendChild(passwordInput);
	connectModal.appendChild(document.createElement("br"));
	connectModal.appendChild(closeButton);
	connectModal.appendChild(connectButton);
	document.body.appendChild(connectModal);
}

function showWifi() {
	const wifi_list = document.querySelector(".wifi-list");
	let table = generateWifiHtml();
	wifi_list.innerHTML = table;
}
