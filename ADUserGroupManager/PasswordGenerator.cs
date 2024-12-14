using System;

namespace ADUserGroupManager
{
    public class PasswordGenerator
    {
        private static readonly string[] words = new string[]
        {
            "metal", "fruit", "horse", "pencil", "bus", "car", "fish", "bike", "city", "apple", "orange", "banana",
            "pear", "grape", "peach", "plum", "kiwi", "lemon", "lime", "berry", "melon", "cherry", "strawberry",
            "raspberry", "blueberry", "mango", "pineapple", "coconut", "papaya", "avocado", "broccoli", "carrot",
            "cucumber", "lettuce", "pepper", "spinach", "tomato", "potato", "onion", "garlic", "ginger", "radish",
            "pumpkin", "squash", "zucchini", "bean", "pea", "corn", "wheat", "oat", "barley", "rice", "quinoa",
            "almond", "peanut", "walnut", "cashew", "pistachio", "hazelnut", "pecan", "macadamia", "chestnut",
            "sashimi", "sushi", "nigiri", "roll", "tempura", "teriyaki", "yakitori", "udon", "ramen", "miso",
            "tofu", "soy", "sauce", "vinegar", "mustard", "ketchup", "mayonnaise", "butter", "cheese", "cream",
            "yogurt", "milk", "bread", "toast", "cereal", "bacon", "sausage", "ham", "turkey", "chicken", "beef",
            "pork", "lamb", "fish", "shrimp", "lobster", "crab", "clam", "oyster", "mussel", "octopus", "squid",
            "tuna", "salmon", "trout", "bass", "cod", "herring", "sardine", "anchovy", "mackerel", "shark",
            "whale", "dolphin", "seal", "walrus", "penguin", "polar", "bear", "tiger", "lion", "leopard", "panther",
            "jaguar", "cheetah", "elephant", "rhino", "hippo", "giraffe", "zebra", "kangaroo", "koala", "panda",
            "monkey", "ape", "gorilla", "chimp", "baboon", "orangutan", "lemur", "sloth", "anteater", "armadillo",
            "porcupine", "beaver", "otter", "seal", "wolf", "coyote", "fox", "deer", "moose", "elk", "caribou",
            "bison", "buffalo", "horse", "donkey", "mule", "camel", "llama", "alpaca", "sheep", "goat", "cow",
            "bull", "ox", "yak", "antelope", "gazelle"
            // Puedes agregar más palabras aquí o cargarlas dinámicamente si lo prefieres.
        };

        private static readonly string[] separators = { "*", "-", ".", ";" };

        public static string GeneratePassword()
        {
            Random random = new Random();
            string word1 = Capitalize(words[random.Next(words.Length)]);
            string word2 = Capitalize(words[random.Next(words.Length)]);
            string word3 = Capitalize(words[random.Next(words.Length)]);
            string separator1 = separators[random.Next(separators.Length)];
            string separator2 = separators[random.Next(separators.Length)];

            return $"{word1}{separator1}{word2}{separator2}{word3}";
        }

        private static string Capitalize(string word)
        {
            if (string.IsNullOrEmpty(word))
                return word;
            return char.ToUpper(word[0]) + word.Substring(1);
        }
    }
}
