# Neuri

Простенькая библиотека на C# для работы с многослойный персептроном, использующим нелинейные персептроны с сигмоидом в качестве функции активации. 

Реализовано обучения градиентным спуском.

Работающий пример запуска 
  Sample.cs
  
# Краткий пример использования
            var network = new SigmoidNetwork(new List<int> { 20, 150, 1 }) // Один скрытый слой в 150 нейронов
                .Randomize();
            network.Learning(eta, epochs, questions, answers);
            var ans = network.Ask(question);
