// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Aguarda o documento carregar
document.addEventListener("DOMContentLoaded", function () {

    // Máscara de telefone celular brasileiro (99) 99999-9999
    const telefoneInputs = document.querySelectorAll(".telefone-mask");

    telefoneInputs.forEach(function (input) {
        input.addEventListener("input", function (e) {
            let value = e.target.value.replace(/\D/g, ''); // Remove tudo que não é número

            if (value.length <= 11) {
                // Aplica a máscara conforme o usuário digita
                if (value.length <= 2) {
                    value = value.replace(/^(\d{0,2})/, '($1');
                } else if (value.length <= 7) {
                    value = value.replace(/^(\d{2})(\d{0,5})/, '($1) $2');
                } else {
                    value = value.replace(/^(\d{2})(\d{5})(\d{0,4})/, '($1) $2-$3');
                }

                e.target.value = value;
            } else {
                // Limita a 14 caracteres formatados: (99) 99999-9999
                e.target.value = e.target.value.slice(0, 15);
            }
        });

        // Remove a máscara ao colar texto
        input.addEventListener("paste", function (e) {
            setTimeout(function () {
                let value = e.target.value.replace(/\D/g, '');
                if (value.length === 11) {
                    e.target.value = value.replace(/^(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
                }
            }, 10);
        });
    });

    // Substitua "Cep" pelo ID correto do seu campo de input no HTML
    // No ASP.NET MVC, o ID gerado costuma ter o mesmo nome da propriedade do ViewModel
    const cepInput = document.getElementById("Cep");

    if (cepInput) {
        // O evento "blur" dispara quando o usuário clica fora do campo de CEP
        cepInput.addEventListener("blur", function () {

            // Remove tudo o que não é número do valor digitado
            let cep = this.value.replace(/\D/g, '');

            // Só faz a pesquisa se o CEP tiver exatamente 8 números
            if (cep.length === 8) {

                // Opcional: Colocar um texto de "..." nos campos enquanto carrega
                document.getElementById("Rua").value = "...";
                document.getElementById("Bairro").value = "...";
                document.getElementById("Cidade").value = "...";
                document.getElementById("Estado").value = "...";

                // Chama a API do ViaCEP
                fetch(`https://viacep.com.br/ws/${cep}/json/`)
                    .then(response => response.json())
                    .then(data => {
                        if (!data.erro) {
                            // Sucesso! Preenche os campos do formulário
                            // ATENÇÃO: Ajuste os IDs ("Rua", "Bairro", etc) para os IDs exatos das suas Views
                            document.getElementById("Rua").value = data.logradouro;
                            document.getElementById("Bairro").value = data.bairro;
                            document.getElementById("Cidade").value = data.localidade;
                            document.getElementById("Estado").value = data.uf;

                            // Se tiver um campo para o IBGE ou complemento, pode preencher também:
                            // document.getElementById("Ibge").value = data.ibge;
                        } else {
                            // Limpa os campos se o CEP for inválido
                            limparCamposEndereco();
                            alert("CEP não encontrado. Verifique o número digitado.");
                        }
                    })
                    .catch(error => {
                        console.error("Erro ao consultar o ViaCEP:", error);
                        limparCamposEndereco();
                        alert("Não foi possível consultar o CEP no momento.");
                    });
            } else {
                limparCamposEndereco();
            }
        });
    }

    // Função auxiliar para limpar os campos caso dê erro
    function limparCamposEndereco() {
        if (document.getElementById("Rua")) document.getElementById("Rua").value = "";
        if (document.getElementById("Bairro")) document.getElementById("Bairro").value = "";
        if (document.getElementById("Cidade")) document.getElementById("Cidade").value = "";
        if (document.getElementById("Estado")) document.getElementById("Estado").value = "";
    }
});