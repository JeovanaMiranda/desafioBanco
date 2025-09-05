using Microsoft.Data.Sqlite;


public class Transacao
{
    public int ContaOrigem { get; private set; }
    public int ContaDestino { get; private set; }
    public double ValorTransacao { get; private set; }
    public int Tipo { get; private set; }
    public DateTime dataHora { get; private set; }

    public void Transferir(int contaOrigem, int contaDestino, double valorTransacao, int escolha, SqliteConnection connection)
    {
        try
        {
            // Atualiza os atributos da classe
            ContaOrigem = contaOrigem;
            ContaDestino = contaDestino;
            ValorTransacao = valorTransacao;
            Tipo = escolha;

            //seleciona o valor do saldo e o titular da conta que envia
            var cmdOrigem = connection.CreateCommand();
            cmdOrigem.CommandText = @"
                SELECT saldo, nomeTitular 
                FROM Conta 
                WHERE numeroConta = $contaOrigem
            ";
            cmdOrigem.Parameters.AddWithValue("$contaOrigem", ContaOrigem);

            var retorno = cmdOrigem.ExecuteReader();
            double saldoOrigem = 0;
            string? nomeOrigem = "";

            try
            {
                if (!retorno.Read())
                {
                    Console.WriteLine($"Conta de origem {ContaOrigem} não encontrada.");
                    return;
                }
                while (retorno.Read())
                {
                    saldoOrigem = retorno.GetDouble(0);
                    nomeOrigem = retorno.GetString(1);
                }
            }
            finally
            {
                retorno.Close();
                retorno.DisposeAsync();
            }

            //seleciona o valor saldo da conta que recebe
            var cmdDestino = connection.CreateCommand();
            cmdDestino.CommandText = @"
                SELECT saldo, nomeTitular 
                FROM Conta 
                WHERE numeroConta = $contaDestino
            ";
            cmdDestino.Parameters.AddWithValue("$contaDestino", ContaDestino);

            var retornoDestino = cmdDestino.ExecuteReader();
            double saldoDestino = 0;
            string? nomeDestino = "";

            try
            {

                if (!retornoDestino.Read())
                {
                    Console.WriteLine($"Conta de destino {ContaDestino} não encontrada.");
                    return;
                }
                while (retornoDestino.Read())
                {
                    saldoDestino = retornoDestino.GetDouble(0);
                    nomeDestino = retornoDestino.GetString(1);
                }
            }
            finally
            {
                retornoDestino.Close();
                retornoDestino.DisposeAsync();
            }

            //testa se saldo é suficiente
            if (saldoOrigem - ValorTransacao > 0)
            {
                //atualiza o saldo da conta que recebe a transações
                var cmdUpdate1 = connection.CreateCommand();
                double novoSaldoDestino = saldoDestino + ValorTransacao;
                double novoSaldoOrigem = saldoOrigem - ValorTransacao;
                cmdUpdate1.CommandText = @"
                    UPDATE Conta SET saldo = $novoSaldoDestino 
                    WHERE numeroConta = $contaDestino   
                ";
                cmdUpdate1.Parameters.AddWithValue("$novoSaldoDestino", novoSaldoDestino);
                cmdUpdate1.Parameters.AddWithValue("$contaDestino", ContaDestino);
                cmdUpdate1.ExecuteNonQuery();

                // atualiza saldo da conta que enviou
                var cmdUpdate2 = connection.CreateCommand();
                cmdUpdate2.CommandText = @"
                    UPDATE Conta SET saldo = $novoSaldoOrigem 
                    WHERE numeroConta = $contaOrigem 
                ";
                cmdUpdate2.Parameters.AddWithValue("$novoSaldoOrigem", novoSaldoOrigem);
                cmdUpdate2.Parameters.AddWithValue("$contaOrigem", ContaOrigem);
                cmdUpdate2.ExecuteNonQuery();

                //Cria nova transação
                DateTime dataHora = DateTime.Now;
                var cmdInsert = connection.CreateCommand();
                cmdInsert.CommandText = @"
                    INSERT INTO Transacao (contaOrigem, contaDestino, tipo, dataHora, valor) 
                    VALUES ($contaOrigem, $contaDestino, $escolha, $dataHora, $valor)    
                ";
                cmdInsert.Parameters.AddWithValue("$contaOrigem", ContaOrigem);
                cmdInsert.Parameters.AddWithValue("$contaDestino", ContaDestino);
                cmdInsert.Parameters.AddWithValue("$escolha", Tipo);
                cmdInsert.Parameters.AddWithValue("$dataHora", dataHora);
                cmdInsert.Parameters.AddWithValue("$valor", ValorTransacao);
                cmdInsert.ExecuteNonQuery();

                Console.WriteLine("\nTRANSFERÊNCIA REALIZADA COM SUCESSO!!\n");
                Console.WriteLine($"VALOR: {ValorTransacao}");
                Console.WriteLine($"DE: {nomeOrigem}");
                Console.WriteLine($"PARA: {nomeDestino}");
                Console.WriteLine($"Momento da transação: {dataHora}");
            }
            else
            {
                Console.WriteLine("Saldo insuficiente para a Transação");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao realizar transferência: {ex.Message}");
        }
    }

    public void Depositar(int contaOrigem, double valorTransacao, int escolha, SqliteConnection connection)
    {
        try
        {
            ContaOrigem = contaOrigem;
            ValorTransacao = valorTransacao;
            Tipo = escolha;

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT saldo FROM Conta WHERE numeroConta = $contaOrigem";
            cmd.Parameters.AddWithValue("$contaOrigem", ContaOrigem);

            object? retorno = cmd.ExecuteScalar();
            if (retorno == null)
            {
                Console.WriteLine($"Conta {ContaOrigem} não encontrada.");
                return;
            }
            double saldoAtual = retorno != null ? Convert.ToDouble(retorno) : 0;
            double novoSaldo = saldoAtual + ValorTransacao;

            var cmdAtualizar = connection.CreateCommand();
            cmdAtualizar.CommandText = "UPDATE Conta SET saldo = $novoSaldo WHERE numeroConta = $contaOrigem";
            cmdAtualizar.Parameters.AddWithValue("$novoSaldo", novoSaldo);
            cmdAtualizar.Parameters.AddWithValue("$contaOrigem", ContaOrigem);
            cmdAtualizar.ExecuteNonQuery();

            DateTime dataHora = DateTime.Now;
            var cmdInsert = connection.CreateCommand();
            cmdInsert.CommandText = @"
                INSERT INTO Transacao (contaOrigem, tipo, dataHora, valor) 
                VALUES ($contaOrigem, $escolha, $dataHora, $valor)    
            ";
            cmdInsert.Parameters.AddWithValue("$contaOrigem", ContaOrigem);
            cmdInsert.Parameters.AddWithValue("$escolha", Tipo);
            cmdInsert.Parameters.AddWithValue("$dataHora", dataHora);
            cmdInsert.Parameters.AddWithValue("$valor", ValorTransacao);
            cmdInsert.ExecuteNonQuery();

            var cmdSelect = connection.CreateCommand();
            cmdSelect.CommandText = "SELECT nomeTitular FROM Conta WHERE numeroConta = $contaOrigem";
            cmdSelect.Parameters.AddWithValue("$contaOrigem", ContaOrigem);

            object? retorno2 = cmdSelect.ExecuteScalar();
            if (retorno2 != null)
            {
                string? nome = Convert.ToString(retorno2);
                Console.WriteLine("\n================DADOS DA TRANSAÇÃO================\n");
                Console.WriteLine($"NUMERO DA CONTA: {ContaOrigem}\nNOME DO TITULAR: {nome}\nTIPO DA TRANSAÇÃO: Depósito\nVALOR DA TRANSAÇÃO: {ValorTransacao}\nSALDO ANTIGO: {saldoAtual}\nSALDO ATUAL: {novoSaldo}");
                Console.WriteLine($"Momento da transação: {dataHora}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao realizar depósito: {ex.Message}");
        }
    }

    public void Sacar(int contaOrigem, double valorTransacao, int escolha, SqliteConnection connection)
    {
        try
        {
            ContaOrigem = contaOrigem;
            ValorTransacao = valorTransacao;
            Tipo = escolha;

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT saldo FROM Conta WHERE numeroConta = $contaOrigem";
            cmd.Parameters.AddWithValue("$contaOrigem", ContaOrigem);

            object? retorno = cmd.ExecuteScalar();

            if (retorno == null)
            {
                Console.WriteLine($"Conta {ContaOrigem} não encontrada.");
                return;
            }
            double saldoAtual = Convert.ToDouble(retorno);

            if (saldoAtual - ValorTransacao >= 0)
            {
                double novoSaldo = saldoAtual - ValorTransacao;

                var cmdUpdate = connection.CreateCommand();
                cmdUpdate.CommandText = "UPDATE Conta SET saldo = $novoSaldo WHERE numeroConta = $contaOrigem";
                cmdUpdate.Parameters.AddWithValue("$contaOrigem", ContaOrigem);
                cmdUpdate.Parameters.AddWithValue("$novoSaldo", novoSaldo);
                cmdUpdate.ExecuteNonQuery();

                DateTime dataHora = DateTime.Now;
                var cmdInsert = connection.CreateCommand();
                cmdInsert.CommandText = @"
                    INSERT INTO Transacao (contaDestino, contaOrigem, tipo, dataHora, valor) 
                    VALUES ($contaDestino, $contaOrigem, $escolha, $dataHora, $valor)    
                ";
                cmdInsert.Parameters.AddWithValue("$contaDestino", ContaOrigem);
                cmdInsert.Parameters.AddWithValue("$contaOrigem", ContaOrigem);
                cmdInsert.Parameters.AddWithValue("$escolha", Tipo);
                cmdInsert.Parameters.AddWithValue("$dataHora", dataHora);
                cmdInsert.Parameters.AddWithValue("$valor", ValorTransacao);
                cmdInsert.ExecuteNonQuery();

                var cmdSel = connection.CreateCommand();
                cmdSel.CommandText = "SELECT nomeTitular FROM Conta WHERE numeroConta = $contaDestino";
                cmdSel.Parameters.AddWithValue("$contaDestino", ContaOrigem);

                object? retorno4 = cmdSel.ExecuteScalar();
                if (retorno4 != null)
                {
                    string? nome = Convert.ToString(retorno4);
                    Console.WriteLine("\n================DADOS DA TRANSAÇÃO================\n");
                    Console.WriteLine($"NUMERO DA CONTA: {ContaOrigem}\nNOME DO TITULAR: {nome}\nTIPO DA TRANSAÇÃO: Saque\nVALOR DA TRANSAÇÃO: {ValorTransacao}\nSALDO ANTIGO: {saldoAtual}\nSALDO ATUAL: {novoSaldo}");
                }
            }
            else
            {
                Console.WriteLine("Saldo insuficiente");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao realizar saque: {ex.Message}");
        }
    }

    public void ConsultarHistorico(int conta, SqliteConnection connection)
    {
        try
        {
            // ================= Transações enviadas =================
            var cmdEnviadas = connection.CreateCommand();
            cmdEnviadas.CommandText = @"
                SELECT tipo, valor, contaDestino, dataHora 
                FROM Transacao 
                WHERE contaOrigem = $conta
            ";
            cmdEnviadas.Parameters.AddWithValue("$conta", conta);

            var readerEnviadas = cmdEnviadas.ExecuteReader();
            if (!readerEnviadas.HasRows)
            {
                Console.WriteLine("Nenhuma transação enviada encontrada.");
            }

            Console.WriteLine("================EXTRATO==================");
            try
            {
                while (readerEnviadas.Read())
                {
                    int tipo = readerEnviadas.GetInt32(0);
                    double valor = readerEnviadas.GetDouble(1);
                    int contaDestino = readerEnviadas.IsDBNull(2) ? 0 : readerEnviadas.GetInt32(2);
                    DateTime dataHora = DateTime.Parse(readerEnviadas.GetString(3));

                    string nomeDestino = "Não aplicável";
                    if (contaDestino != 0)
                    {
                        var cmdNomeDest = connection.CreateCommand();
                        cmdNomeDest.CommandText = "SELECT nomeTitular FROM Conta WHERE numeroConta = $contaDestino";
                        cmdNomeDest.Parameters.AddWithValue("$contaDestino", contaDestino);
                        var readerNomeDest = cmdNomeDest.ExecuteReader();
                        if (readerNomeDest.Read())
                        {
                            nomeDestino = readerNomeDest.GetString(0);
                        }
                        readerNomeDest.Close();
                    }

                    if (tipo == 2) Console.WriteLine($"Depósito realizado\nValor: {valor}\nData: {dataHora}");
                    else if (tipo == 3) Console.WriteLine($"Saque realizado\nValor: {valor}\nData: {dataHora}");
                    else if (tipo == 4) Console.WriteLine($"Transferência realizada para: {nomeDestino}\nValor: {valor}\nData: {dataHora}");
                }
            }
            finally
            {
                readerEnviadas.Close();
                readerEnviadas.DisposeAsync();
            }

            // ================= Transações recebidas =================
            var cmdRecebidas = connection.CreateCommand();
            cmdRecebidas.CommandText = @"
                SELECT tipo, valor, contaOrigem, dataHora
                FROM Transacao 
                WHERE contaDestino = $conta
                AND tipo = 4
            ";
            cmdRecebidas.Parameters.AddWithValue("$conta", conta);

            var readerRecebidas = cmdRecebidas.ExecuteReader();

            try
            {
                while (readerRecebidas.Read())
                {
                    int tipo = readerRecebidas.GetInt32(0);
                    double valor = readerRecebidas.GetDouble(1);
                    int contaOrigem = readerRecebidas.IsDBNull(2) ? 0 : readerRecebidas.GetInt32(2);
                    DateTime dataHora = DateTime.Parse(readerRecebidas.GetString(3));

                    string nomeOrigem = "Não aplicável";
                    if (contaOrigem != 0)
                    {
                        var cmdNomeOrig = connection.CreateCommand();
                        cmdNomeOrig.CommandText = "SELECT nomeTitular FROM Conta WHERE numeroConta = $contaOrigem";
                        cmdNomeOrig.Parameters.AddWithValue("$contaOrigem", contaOrigem);
                        var readerNomeOrig = cmdNomeOrig.ExecuteReader();
                        if (readerNomeOrig.Read())
                        {
                            nomeOrigem = readerNomeOrig.GetString(0);
                        }
                        readerNomeOrig.Close();
                    }

                    Console.WriteLine($"Transferência recebida de: {nomeOrigem}\nValor: {valor}\n Data: {dataHora}");
                }
            }
            finally
            {
                readerRecebidas.Close();
                readerRecebidas.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao consultar histórico: {ex.Message}");
        }
    }
}
