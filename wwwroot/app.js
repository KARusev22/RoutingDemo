// Базов URL на нашия REST endpoint (роута на TasksController).
const API = "/api/tasks";

const tbody = document.querySelector("#logTable tbody");

// Добавя ред в дневника: метод, URL, статус код, час.
function logRequest(method, url, status) {
    const tr = document.createElement("tr");
    const time = new Date().toLocaleTimeString();
    const cls = status >= 200 && status < 300 ? "status-ok" : "status-err";

    tr.innerHTML =
        `<td><span class="verb ${method.toLowerCase()}">${method}</span></td>` +
        `<td>${escapeHtml(url)}</td>` +
        `<td class="${cls}">${status}</td>` +
        `<td>${time}</td>`;

    tbody.prepend(tr);
}

// GET /api/tasks — изтегля всички задачи от сървъра и ги рисува.
async function loadTasks() {
    const res = await fetch(API);
    logRequest("GET", API, res.status);

    const tasks = await res.json();
    const list = document.getElementById("taskList");
    list.innerHTML = "";
    document.getElementById("empty").hidden = tasks.length > 0;

    for (const task of tasks) {
        const li = document.createElement("li");

        const text = document.createElement("span");
        text.textContent = `${task.id}. ${task.title}`;

        const btn = document.createElement("button");
        btn.textContent = "Изтрий";
        btn.className = "delete";
        btn.addEventListener("click", () => deleteTask(task.id));

        li.appendChild(text);
        li.appendChild(btn);
        list.appendChild(li);
    }
}

// POST /api/tasks — изпраща JSON с заглавието на новата задача.
async function addTask(title) {
    const res = await fetch(API, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ title: title })
    });

    logRequest("POST", API, res.status);

    if (res.ok) {
        await loadTasks();
    } else {
        alert("Грешка при добавяне: HTTP " + res.status);
    }
}

// DELETE /api/tasks/{id} — изтрива задача по нейния id.
async function deleteTask(id) {
    const url = `${API}/${id}`;
    const res = await fetch(url, { method: "DELETE" });

    logRequest("DELETE", url, res.status);

    if (res.ok) {
        await loadTasks();
    } else {
        alert("Грешка при изтриване: HTTP " + res.status);
    }
}

// Защита срещу HTML injection при показване на потребителски текст.
function escapeHtml(text) {
    const div = document.createElement("div");
    div.textContent = text;
    return div.innerHTML;
}

// Обработка на формата: при submit → POST.
document.getElementById("addForm").addEventListener("submit", (e) => {
    e.preventDefault();
    const input = document.getElementById("title");
    const value = input.value.trim();
    if (value) {
        addTask(value);
        input.value = "";
    }
});

// Бутон "Презареди" → GET.
document.getElementById("refresh").addEventListener("click", loadTasks);

// Зареждаме списъка при стартиране на страницата.
loadTasks();
