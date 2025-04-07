document.addEventListener("DOMContentLoaded", function () {
    var btnProcessar = document.getElementById("btnProcessar");
    if (btnProcessar) {
        btnProcessar.addEventListener("click", function (event) {
            event.preventDefault(); // Evita o envio normal do formulário

            var formData = new FormData(document.querySelector("form"));

            fetch('/Convenio/ProcessarArquivo', {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    console.log("Resposta da API ProcessarArquivo:", data);
                    if (data.success) {
                        console.log("Chamando buscarValoresCcoluna1()");
                        buscarValoresCcoluna1(); // Abre o primeiro modal
                    } else {
                        alert(data.message);
                    }
                })
                .catch(error => console.error('Erro ao processar arquivos:', error));
        });
    }
});

//<< =========================  Modal Ccoluna1 ===========================>>

function buscarValoresCcoluna1() {
    fetch('/Convenio/ObterValoresCcoluna1')
        .then(response => response.json())
        .then(data => {
            const container = document.getElementById("valoresCcoluna1Container");
            container.innerHTML = "";

            data.forEach(valor => {
                container.innerHTML += `
                    <div class="mb-1">
                        <label class="form-label">Secretaria: ${valor}</label>
                        <input type="text" class="form-control novoValor" data-original="${valor}" placeholder="${valor}">
                    </div>
                `;
            });

            var modal = new bootstrap.Modal(document.getElementById('modalCcoluna1'));
            modal.show();
        })
        .catch(error => console.error('Erro ao buscar valores de Ccoluna1:', error));
}

function atualizarValoresCcoluna1() {
    let valoresAtualizados = {};

    document.querySelectorAll("#valoresCcoluna1Container input").forEach(input => {
        const original = input.getAttribute("data-original"); // Pegando o valor original de Ccoluna1
        const novoValor = input.value.trim();

        // Só adiciona se o valor foi alterado
        if (novoValor && novoValor !== original) {
            // Envia a alteração para Ccoluna21, mantendo Ccoluna1 como referência
            valoresAtualizados[original] = novoValor;
        }
    });

    if (Object.keys(valoresAtualizados).length === 0) {
        alert("Nenhuma alteração foi feita.");
        return;
    }

    fetch('/Convenio/AtualizarValoresCcoluna21', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(valoresAtualizados)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Fechar o modal
                var modal = bootstrap.Modal.getInstance(document.getElementById('modalCcoluna1'));
                modal.hide();

                // Abrir o próximo modal ou realizar outras ações necessárias
                abrirModalCargo(); // Exemplo de abrir o próximo modal
            } else {
                alert("Erro ao atualizar valores: " + data.message);
            }
        })
        .catch(error => console.error('Erro ao atualizar valores de Ccoluna1:', error));
}

//<< =========================  Modal CARGO ===========================>>

function buscarValoresCargo() {
    fetch('/Convenio/ObterValoresCargo')
        .then(response => response.json())
        .then(data => {
            const container = document.getElementById("valoresCargoContainer");
            container.innerHTML = "";

            data.forEach(valor => {
                container.innerHTML += `
                    <div class="mb-1">
                        <label class="form-label">Cargo: ${valor}</label>
                        <input type="text" class="form-control novoCargo" data-original="${valor}" placeholder="${valor}">
                    </div>
                `;
            });

            var modal = new bootstrap.Modal(document.getElementById('modalCargo'));
            modal.show();
        })
        .catch(error => console.error('Erro ao buscar valores de Cargo:', error));
}

function atualizarValoresCargo() {
    const valoresAtualizados = {};
    document.querySelectorAll(".novoCargo").forEach(input => {
        const original = input.getAttribute("data-original");
        const novoValor = input.value.trim();

        if (novoValor && novoValor !== original) {
            valoresAtualizados[original] = novoValor;
        }
    });

    if (Object.keys(valoresAtualizados).length === 0) {
        alert("Nenhuma alteração foi feita.");
        return;
    }

    fetch('/Convenio/AtualizarValoresCcoluna21', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(valoresAtualizados)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Fechar o modal
                var modal = bootstrap.Modal.getInstance(document.getElementById('modalCargo'));
                modal.hide();
                abrirModalCcoluna16(); // Abrir o modal Ccoluna16
            } else {
                alert("Erro ao atualizar valores: " + data.message);
            }
        })
        .catch(error => console.error('Erro ao atualizar valores de Ccoluna21:', error));
}


//<< =========================  Modal Ccoluna16 ===========================>>

function buscarValoresDistintos() {
    fetch('/Convenio/ObterValoresDistintosCcoluna16')
        .then(response => response.json())
        .then(data => {
            console.log('Dados de Ccoluna16:', data);  // Adicione esta linha para verificar os dados
            const container = document.getElementById("valoresContainer");

            if (!container) {
                console.error("Elemento #valoresContainer não encontrado.");
                return;
            }

            container.innerHTML = "";

            data.forEach(valor => {
                container.innerHTML += `
                    <div class="mb-1">
                        <label class="form-label">Vinculo: ${valor}</label>
                        <input type="text" class="form-control novoValor" data-original="${valor}" placeholder="${valor}">
                    </div>
                `;
            });

            var modal = new bootstrap.Modal(document.getElementById('modalCcoluna16'));
            modal.show();
        })
        .catch(error => console.error('Erro ao buscar valores distintos:', error));
}

function atualizarValoresCcoluna16() {
    const valoresAtualizados = {};
    document.querySelectorAll(".novoValor").forEach(input => {
        const original = input.getAttribute("data-original");
        const novoValor = input.value.trim();

        if (novoValor && novoValor !== original) {
            valoresAtualizados[original] = novoValor;
        }
    });

    if (Object.keys(valoresAtualizados).length === 0) {
        alert("Nenhuma alteração foi feita.");
        return;
    }

    fetch('/Convenio/AtualizarValoresCcoluna16', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(valoresAtualizados)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Fechar o modal
                var modal = bootstrap.Modal.getInstance(document.getElementById('modalCcoluna16'));
                modal.hide();
                abrirModalCalculo(); // Abrir o modal Calculo
            } else {
                alert("Erro ao atualizar valores: " + data.message);
            }
        })
        .catch(error => console.error('Erro ao atualizar valores de Calculo:', error));
}

//<< =========================  Modal Calculo ===========================>>

function buscarValoresCalculo() {
    fetch('/Convenio/ObterValoresCalculo')
        .then(response => response.json())
        .then(data => {
            console.log('Dados de Calculo:', data);

            const container = document.getElementById("valoresCalculoContainer");
            if (!container) {
                console.error("Elemento #valoresCalculoContainer não encontrado.");
                return;
            }

            container.innerHTML = "";

            data.forEach(valor => {
                container.innerHTML += `
                    <div class="mb-1">
                        <label class="form-label">Perfil Cálculo: ${valor}</label>
                        <input type="text" class="form-control novoValor" data-original="${valor}" placeholder="${valor}">
                    </div>
                `;
            });

            var modal = new bootstrap.Modal(document.getElementById('modalCalculo'));
            modal.show();
        })
        .catch(error => console.error('Erro ao buscar valores distintos:', error));
}

function atualizarValoresCcoluna18() {
    const valoresAtualizados = {};
    document.querySelectorAll(".novoValor").forEach(input => {
        const original = input.getAttribute("data-original");
        const novoValor = input.value.trim();

        if (novoValor && novoValor !== original) {
            valoresAtualizados[original] = novoValor;
        }
    });

    console.log("Valores a serem atualizados:", valoresAtualizados); // Adicionando LOG para depuração

    if (Object.keys(valoresAtualizados).length === 0) {
        alert("Nenhuma alteração foi feita.");
        return;
    }

    fetch('/Convenio/AtualizarValoresCcoluna18', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(valoresAtualizados)
    })
        .then(response => response.json())
        .then(data => {
            console.log("Resposta do servidor:", data); // Adicionando LOG para verificar retorno do backend

            if (data.success) {
                var modal = bootstrap.Modal.getInstance(document.getElementById('modalCalculo'));
                modal.hide();

                // Chamar o backend para buscar as quantidades de discrepâncias
                fetch('/Convenio/ObterQuantidadeDiscrepancias')
                    .then(response => response.json())
                    .then(discrepancias => {
                        const container = document.getElementById("discrepanciasContainer");
                        container.innerHTML = `
                            <p><strong>Resumo de Discrepâncias Geradas:</strong></p>
                            <ul>
                                <li>SERVIDOR: ${discrepancias.servidor} linha(s)</li>
                                <li>MATRÍCULA: ${discrepancias.matricula} linha(s)</li>
                                <li>CATEGORIA: ${discrepancias.categoria} linha(s)</li>
                                <li>SECRETARIA: ${discrepancias.secretaria} linha(s)</li>
                                <li>PERFIL CÁLCULO: ${discrepancias.perfilCalculo} linha(s)</li>
                            </ul>
                        `;

                        var modalResumo = new bootstrap.Modal(document.getElementById('modalDiscrepancias'));
                        modalResumo.show();
                    })
                    .catch(error => {
                        console.error('Erro ao buscar discrepâncias:', error);
                        alert('Erro ao obter resumo de discrepâncias.');
                    });
            }
        });
}

window.addEventListener('DOMContentLoaded', () => {
    fetch('/SelectOption/Preencher', {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            console.log("Tabela SelectOption foi preenchida:", data);
        })
        .catch(error => {
            console.error("Erro ao preencher tabela:", error);
        });
});
//<< =========================  Controle de fluxo dos Modais ===========================>>

function abrirModalCargo() {
    var modalCcoluna1 = document.getElementById('modalCcoluna1');
    var modalCargo = new bootstrap.Modal(document.getElementById('modalCargo'));

    // Fecha o modal Ccoluna1 e abre o modal Cargo
    var bsModal = bootstrap.Modal.getInstance(modalCcoluna1);
    bsModal.hide();

    modalCcoluna1.addEventListener('hidden.bs.modal', function () {
        buscarValoresCargo(); // Carregar os valores de Cargo antes de abrir a modal
        modalCargo.show();
    }, { once: true });
}

function abrirModalCcoluna16() {
    var modalCargo = document.getElementById('modalCargo');
    var modalCcoluna16 = new bootstrap.Modal(document.getElementById('modalCcoluna16'));

    // Fecha o modal atual e aguarda antes de abrir o próximo
    var bsModal = bootstrap.Modal.getInstance(modalCargo);
    bsModal.hide();

    // Garantir que a busca de valores seja concluída antes de abrir o próximo modal
    modalCargo.addEventListener('hidden.bs.modal', function () {
        buscarValoresDistintos(); // Carregar os valores de Ccoluna16 antes de abrir o modal
        modalCcoluna16.show();
    }, { once: true });
}

function abrirModalCalculo() {
    // Fechar o modal atual (modalCcoluna16)
    var modalCcoluna16 = bootstrap.Modal.getInstance(document.getElementById('modalCcoluna16'));
    if (modalCcoluna16) modalCcoluna16.hide();

    // Buscar valores e abrir modalCalculo
    buscarValoresCalculo();
}


