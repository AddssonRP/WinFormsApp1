# SexVip

## Instalador Windows

Para gerar o instalador MSI x64, execute `Installer\build-installer.cmd` no Explorador de Arquivos ou no Prompt de Comando. O arquivo gerado fica em `Installer\bin\Release\SexVip.Installer.msi`.

O script publica o aplicativo como executável autocontido e compila o MSI usando WiX Toolset 5. A primeira execução precisa de acesso ao NuGet para restaurar o WiX SDK. O MSI instala o aplicativo no perfil do usuário, cria atalhos no Menu Iniciar e na Área de Trabalho (usando o ícone embutido no executável) e registra a opção de desinstalação do Windows. A instalação não requer permissão de administrador.

O pacote ainda não é assinado digitalmente. Para distribuição pública, assine o instalador com um certificado de assinatura de código confiável, para reduzir os avisos do Windows SmartScreen.