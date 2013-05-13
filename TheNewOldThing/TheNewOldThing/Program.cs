using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

//TODO: Put these classes into separate files for maintainability. Leaving as-is for readability.
//TODO: Add code to handle invalid input (file name, mal-content, etc)
//TODO: Add parameter checking or Constraints
namespace TheNewOldThing
{
    class Program
    {
        static void Main(string[] args)
        {
            // Possible future requirements to consider:
            //     - Gracefully handle other kinds of exceptions
            //      
            try
            {
                OutputPipe
                    .To(Console.Out)
                    .Write(Algorithm.DistinctCounts(InputPipe.From(new StreamReader(args[0])).Read()));
            }
            catch (FileNotFoundException fnf)
            {
                Console.WriteLine("The file could not be found.");
            }
            catch (Exception)
            {
                Console.WriteLine("We apologize but something we didn't expect has happened and this program cannot continue.");
            }

            Console.ReadLine();
        }
    }

    public class InputPipe
    {
        public static InputPipe From(TextReader reader)
        {
            return new InputPipe(reader);
        }

        public IEnumerable<string> Read()
        {
            string line;
            // Expects a specific format.
            // Possible future requirements to consider:
            //     - Gracefully handle bad input (e.g. no comma)
            //      
            while ((line = _reader.ReadLine()) != null)
                yield return line.Split(',')[1];
        }

        protected InputPipe(TextReader reader)
        {
            _reader = reader;
        }

        private readonly TextReader _reader;
    }

    public class Algorithm
    {
        // TODO: Consider allowing parallel optimization to be configurable.
        public static IEnumerable<Tuple<string, int>> DistinctCounts(IEnumerable<string> items)
        {
            var summary = new ConcurrentDictionary<string, int>();
            Parallel.ForEach(items, item => summary.AddOrUpdate(item, 1, (k, v) => v + 1));
            return summary.Select(item => Tuple.Create(item.Key, item.Value));
        }
    }

    public class OutputPipe
    {
        public static OutputPipe To(TextWriter writer)
        {
            return new OutputPipe(writer);
        }

        public int Write(IEnumerable<Tuple<string, int>> items)
        {
            foreach (var item in items)
            {
                _writer.WriteLineAsync(String.Format("{0},{1}", item.Item1, item.Item2));
            }
            
            return items.Count();
        }

        protected OutputPipe(TextWriter writer)
        {
            _writer = writer;
        }

        private readonly TextWriter _writer;
    }

    public class given_an_input_pipe
    {
        protected readonly InputPipe _sut;

        public given_an_input_pipe()
        {
            _sut = InputPipe.From(File.OpenText(@"..\..\TestData.csv"));
        }

        [Fact]
        public void when_reading_from_well_formed_file_then_enumerate()
        {
            //var reader = new StringReader("2,30");
            //reader.ReadLineAsync();
            foreach (var item in _sut.Read())
            {
                //Assert.Equal(item.);
            }
        }
    }

    public class given_an_output_pipe
    {
        protected readonly OutputPipe _sut;
        protected readonly StringBuilder _buffer;

        public given_an_output_pipe()
        {
            _buffer = new StringBuilder();
            _sut = OutputPipe.To(new StringWriter(_buffer));
        }

        [Fact]
        public void when_writing_non_empty_list_then_writes_csv_tuple()
        {
            var items = new[] {Tuple.Create("1", 1)};
            _sut.Write(items);
            Assert.Equal("1,1" + Environment.NewLine, _buffer.ToString());
        }
    }

    public class given_a_distinct_counts_algorithm
    {
        protected readonly Func<IEnumerable<string>, IEnumerable<Tuple<string, int>>> _fut;

        public given_a_distinct_counts_algorithm()
        {
            _fut = Algorithm.DistinctCounts;
        }

        [Fact]
        public void when_using_non_repeating_items_then_counts_equal_1()
        {
            var data = Enumerable.Range(1, 1000).Select(item => item.ToString(CultureInfo.InvariantCulture)).ToArray();
            var counts = _fut(data);
            foreach(var tuple in counts)
                Assert.Equal(1, tuple.Item2);
        }
    }
}
