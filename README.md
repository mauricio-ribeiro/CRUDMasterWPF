# CRUDMaster - Sistema de Gerenciamento de Pedidos

## 📝 Descrição
Sistema de gerenciamento de pedidos desenvolvido em C# com WPF, utilizando padrões MVVM e boas práticas de programação orientada a objetos.

## 🛠️ Tecnologias e Dependências
- **Plataforma**: .NET 8.0
- **UI Framework**: WPF
- **Dependência Principal**: Newtonsoft.Json (v13.0.3)
- **Padrão de Arquitetura**: MVVM

## 📂 Estrutura de Dados

bin/Debug/net8.0-windows/Data/
├── pessoas.json # Cadastro de clientes
├── produtos.json # Cadastro de produtos
└── pedidos.json # Registro de pedidos


## 🚀 Instalação e Execução

### Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git (opcional)

```bash
# 1. Clone o repositório
git clone https://github.com/seu-usuario/CRUDMaster.git

# 2. Acesse o diretório
cd CRUDMaster/CRUDMaster

# 3. Restaure os pacotes
dotnet restore

# 4. Execute
dotnet run

Módulos Principais
Módulo	Descrição
Pessoas	Cadastro completo de clientes
Produtos	Registro com nome, valor e quantidades
Pedidos	Criação e gestão com cálculos automáticos

Filtros de Pedidos
✅ Por cliente

✅ Por status (Pendente/Pago/Enviado/Recebido)

✅ Por período de datas


Configurações Técnicas

<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3"/>
  </ItemGroup>
</Project>

Observações

1 - A pasta Data é criada automaticamente

2 - Persistência em JSON local

3 - IDs incrementais automáticos
