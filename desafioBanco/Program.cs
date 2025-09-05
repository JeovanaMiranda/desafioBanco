
//Conta c1 = new();
//c1.CriarConta("Jeovana");

public class Program
{
    static void Main()
    {
        var db = new Database();
        using var conn = db.GetConnection();
        Console.WriteLine("Conexão aberta com sucesso!");

        int escolha;
        string? escolhaStr;


        do
        {
            Console.WriteLine("====================CAIXA ELETRÔNICO=============================");
            Console.WriteLine("01 - Criar Conta");
            Console.WriteLine("02 - Depositar");
            Console.WriteLine("03 - Sacar");
            Console.WriteLine("04 - Transferir");
            Console.WriteLine("05 - Consultar Saldo");
            Console.WriteLine("06 - Consultar Histórico");
            Console.WriteLine("00 - Sair");

            Console.WriteLine("O que deseja fazer? ");
            escolhaStr = Console.ReadLine();
            escolha = int.Parse(escolhaStr);
            if (escolha == 1)
            {
                Console.WriteLine("Digite o nome completo do Titular: ");
                string? nome = Console.ReadLine();
                var conta = new Conta();
                conta.CriarConta(nome, conn);

            }
            else if (escolha == 2)
            {
                Console.WriteLine("Selecione a conta que quer depositar: ");
                string? contaStr = Console.ReadLine();
                Console.WriteLine("Digite o valor que quer depositar: ");
                string? valorStr = Console.ReadLine();
                double valor = double.Parse(valorStr);
                int conta = int.Parse(contaStr);

                Transacao t1 = new();
                t1.Depositar(conta, valor, escolha, conn);

            }
            else if (escolha == 3)
            {
                Console.WriteLine("Selecione a conta que quer sacar: ");
                string? contaStr = Console.ReadLine();

                Console.WriteLine("Digite o valor que quer sacar: ");
                string? valorStr = Console.ReadLine();

                double valor = double.Parse(valorStr);
                int conta = int.Parse(contaStr);

                Transacao d1 = new();
                d1.Sacar(conta, valor, escolha, conn);
            }

            else if (escolha == 4)
            {
                Console.WriteLine("Selecione a conta que irá transferir: ");
                string? contaOrigemStr = Console.ReadLine();

                Console.WriteLine("Selecione a conta que irá receber: ");
                string? contaDestinoStr = Console.ReadLine();

                Console.WriteLine("Digite o valor que quer transferir : ");
                string? valorStr = Console.ReadLine();

                double valor = double.Parse(valorStr);
                int contaO = int.Parse(contaOrigemStr);
                int contaD = int.Parse(contaDestinoStr);

                Transacao tr1 = new();
                tr1.Transferir(contaO, contaD, valor, escolha, conn);

            }
            else if (escolha == 5)
            {
                Console.WriteLine("Selecione a conta que quer olhar o saldo ");
                string? contaStr = Console.ReadLine();
                int conta = int.Parse(contaStr);


                Conta consulta = new();
                consulta.ConsultarSaldo(conta, conn);

            }
            else if (escolha == 6)
            {
                Transacao h1 = new();

                Console.WriteLine("Selecione a conta que quer puxar o histórico: ");
                string? contaStr = Console.ReadLine();
                int conta = int.Parse(contaStr);
                h1.ConsultarHistorico(conta, conn);
            }

        }

        while (escolha != 0);
    }



}