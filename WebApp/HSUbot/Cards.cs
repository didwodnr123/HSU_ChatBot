using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace HSUbot
{
    public static class Cards
    {
        public static Attachment CreateAdaptiveCardAttachment()
        {
            // combine path for cross platform support
            string[] paths = { ".", "Resources", "adaptiveCard.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
        }

        public static HeroCard GetYoonGaNeCard()
        {
            var YoonGaNeCard = new HeroCard
            {
                Title = "윤가네",
                Subtitle = "한식/분식",
                Text = "메뉴가 다양해 남여노소 좋아하는 식당\n\n" +
                "전화번호 : 02-765-7179",
                Images = new List<CardImage> { new CardImage("http://mblogthumb1.phinf.naver.net/20160122_212/neonmacaron_1453441191427eObuL_JPEG/012402.jpg?type=w2") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=752168392"),
                },
            };

            return YoonGaNeCard;
        }

        public static HeroCard GetRiceBurgerCard()
        {
            var RiceBurgerCard = new HeroCard
            {
                Title = "봉구스 밥버거",
                Subtitle = "한식/분식",
                Text = "봉구스(Bongousse)는 맛있는 한입거리라는 뜻의 프랑스어입니다. 봉구스밥버거는 어머니의 정성과 신세대의 입맛에 어우러진 '신개념 주먹밥'입니다. '영양'과 '맛'뿐만 아니라 '저렴한 가격'과 '든든함'으로 맛있는 한 끼를 제공하고자 합니다.\n\n" +
                "전화번호 : 02-741-6535",
                Images = new List<CardImage> { new CardImage("http://ldb.phinf.naver.net/20190413_242/1555133670112DoOrT_JPEG/nIE51ESQFkbNcHlTYE8Oqu6a.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=32430405"),
                },
            };

            return RiceBurgerCard;
        }

        public static HeroCard GetWoonBongCard()
        {
            var WoonBongCard = new HeroCard
            {
                Title = "운봉칼국수/보쌈",
                Subtitle = "한식",
                Text = "칼국수와 보쌈정식이 일품인 식당\n\n" +
                "전화번호 : 02-953-5155",
                Images = new List<CardImage> { new CardImage("https://blogfiles.pstatic.net/MjAxODEwMDVfMjA5/MDAxNTM4NzAwNDk0MTM2.19JolCyDMqo9h-aAzlUIstNFkP52ebPLoDeweTGA_QMg.lR4bJ7re4O17S7Qh-4JteceT7kWhwKtwIMArRgC3jKIg.JPEG.areyeonn/IMG_2688.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=38522466"),
                },
            };

            return WoonBongCard;
        }

        public static HeroCard GetSeunglijangCard()
        {
            var SeunglijangCard = new HeroCard
            {
                Title = "승리장",
                Subtitle = "중식",
                Text = "요일별 할인 메뉴가 있어서 더 맛있는 중국집\n\n" +
                "전화번호 : 02-765-1004",
                Images = new List<CardImage> { new CardImage("http://blogfiles.naver.net/MjAxNzEwMDRfMTM2/MDAxNTA3MDk5NjU5MTEz.6Ud2Anc3au8fbR4NEYb5xdAApasIKl26tryG0yK5qbgg.hUAy4uWPGsrxGKhUUfEluGAjfwskg2jmXjBHPKlAqp8g.JPEG.kofparksm/KakaoTalk_20171004_145206086.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=18897782"),
                },
            };

            return SeunglijangCard;
        }

        public static HeroCard GetJoongHwaMyungGaCard()
        {
            var JoongHwaMyungGaCard = new HeroCard
            {
                Title = "중화명가",
                Subtitle = "중식",
                Text = "배달 전문 중국집\n\n" +
                "전화번호 : 02-924-8260",
                Images = new List<CardImage> { new CardImage("http://blogfiles.naver.net/MjAxOTAzMDFfMTI0/MDAxNTUxMzkzNzkzMzc0.qK0MVUkacKqfgYyntXsolXuLEAB67swytgOZFD60dD8g._R2Sm3ZRKAvG2XHwRD-8RXiGXG6xRNxorX8UPdtX_TQg.JPEG.creedy20/%C4%ED%C6%F9.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=37608983"),
                },
            };

            return JoongHwaMyungGaCard;
        }

        public static HeroCard GetMrDonkkasCard()
        {
            var MrDonkkasCard = new HeroCard
            {
                Title = "미스터돈까스",
                Subtitle = "일식",
                Text = "돈까스 전문신선한 재료로 맛좋은 돈까스를 합리적인 가격에 제공해드립니다! 매장에 들러서 바삭하고 맛있는 돈까스 드셔보세요!\n\n" +
                "전화번호 : 02-743-7417",
                Images = new List<CardImage> { new CardImage("http://ldb.phinf.naver.net/20180523_160/1527063627281CWlus_JPEG/gQAAgjb0yWY2SirMVu4yjrTE.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=1720543607"),
                },
            };

            return MrDonkkasCard;
        }

        public static HeroCard GetSushiHarooCard()
        {
            var SushiHarooCard = new HeroCard
            {
                Title = "스시하루",
                Subtitle = "일식",
                Text = "저희 가게는 음식물을 결코 단 한 가지도 재활용하는 품목이 없습니다. 가격 대비 가성비는 저 스스로 자부할 수 있습니다. 항상 신선하고 깔끔한 음식을 만들고 있습니다.\n\n" +
                "전화번호 : 02-6339-0037",
                Images = new List<CardImage> { new CardImage("http://ldb.phinf.naver.net/20190426_141/1556261900127Xktdl_JPEG/Ix6rWM6UPAIuMFRtEtpGbCeb.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=38681623"),
                },
            };

            return SushiHarooCard;
        }

        public static HeroCard GetSushiHyeonCard()
        {
            var SushiHyeonCard = new HeroCard
            {
                Title = "스시현",
                Subtitle = "일식",
                Text = "성북천에 새로 생긴 초밥 전문집\n\n" +
                "전화번호 : 02-744-2257",
                Images = new List<CardImage> { new CardImage("http://ldb.phinf.naver.net/20161101_150/1478006975737sl92n_JPEG/177051625955509_0.jpeg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?entry=plt&id=37822466&query=%EC%8A%A4%EC%8B%9C%ED%98%84"),
                },
            };

            return SushiHyeonCard;
        }

        public static HeroCard GetStarDongCard()
        {
            var StarDong = new HeroCard
            {
                Title = "스타동",
                Subtitle = "일식",
                Text = "일본식 가정요리\n\n" +
                "전화번호 : 02-743-7707",
                Images = new List<CardImage> { new CardImage("http://ldb.phinf.naver.net/20190714_150/1563090077308vjVM9_JPEG/p_5Llqc4skLRlLwherkFHOz9.jpeg.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=38522520"),
                },
            };

            return StarDong;
        }

        public static HeroCard GetMecaDDuckCard()
        {
            var MecaDDuckCard = new HeroCard
            {
                Title = "매카떡",
                Subtitle = "분식",
                Text = "안녕하세요 매카떡입니다^^ 중독성쩌는 매운카레떡볶이! 한번들려서 맛보세요\n\n" +
                "전화번호 : 02-744-7152",
                Images = new List<CardImage> { new CardImage("http://ldb.phinf.naver.net/20180620_59/15294954131820s2f2_JPEG/1lJbMiBaZP_z5alGl64-z5Ki.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=840370406"),
                },
            };

            return MecaDDuckCard;
        }

        public static HeroCard GetSinJeonCard()
        {
            var SinJeonCard = new HeroCard
            {
                Title = "신전떡볶이",
                Subtitle = "분식",
                Text = "수많은 블로거의 극찬을받은 이름난 매운 떡볶이 신전떡볶이입니다 순한맛부터 매운맛까지 다양한 고객들의 입맛을 사로잡고있는 중독성있는 떡볶이 맛보러오세요 ^ ^포장, 배달 모두가능하니 전화주세요\n\n" +
                "* 최소배달금액 13,000원(거리에 따라 배달금액 변동)\n\n" +
                "* 카드사용가능\n\n" +
                "* 전화번호 : 02-744-7177",
                Images = new List<CardImage> { new CardImage("http://ldb.phinf.naver.net/20170530_71/1496121751230xBEVz_JPEG/186480545249928_0.jpeg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=36620074"),
                },
            };

            return SinJeonCard;
        }

        public static HeroCard GetHoChickenCard()
        {
            var HoChicken = new HeroCard
            {
                Title = "호치킨",
                Subtitle = "치킨",
                Text = "오븐에 한번 더 구워서 바삭함과 소이립 풍미를 살린 프리미엄 치킨\n\n" +
                "전화번호 : 02-744-5727",
                Images = new List<CardImage> { new CardImage("http://ldb.phinf.naver.net/20190131_116/1548899179339krj5X_JPEG/EUz7qOW6RcVf1Ck471eVw21T.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=37165551"),
                },
            };

            return HoChicken;
        }

        public static HeroCard GetPizzaBellCard()
        {
            var PizzaBellCard = new HeroCard
            {
                Title = "피자벨",
                Subtitle = "피자",
                Text = "※ 100% 핸드메이드 PIZZA BELL\n\n" +
                "※건강하고 쫄깃한 흑미도우를 사용하여 100 % 수제 재료로 100 % 수제 조리하는 수제피자 전문점 입니다.\n\n" +
                "전화번호 : 02-749-3369",
                Images = new List<CardImage> { new CardImage("http://ldb.phinf.naver.net/20150901_13/1441118668811ncw7f_JPEG/SUBMIT_1426554957096_35003497.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=35003497"),
                },
            };
            return PizzaBellCard;
        }

        public static HeroCard GetAuneCard()
        {
            var AuneCard = new HeroCard
            {
                Title = "아우네 순대국",
                Subtitle = "순대국",
                Text = "순대국 전문점\n\n" +
                "전화번호 : 02-6082-6565",
                Images = new List<CardImage> { new CardImage("http://ldb.phinf.naver.net/20180817_92/1534487914884J4nm5_JPEG/lNzynY2eoiBZWHU2qxiB_pmo.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=1678888344"),
                },
            };
            return AuneCard;
        }

        public static HeroCard GetDonamgolCard()
        {
            var DonamgolCard = new HeroCard
            {
                Title = "돈암골",
                Subtitle = "순대국 / 감자탕",
                Text = "순대국, 감자탕, 뼈해장국 전문점\n\n" +
                "전화번호 : 02-3672-2324",
                Images = new List<CardImage> { new CardImage("http://ldb.phinf.naver.net/20190618_146/1560840677319NOEB3_JPEG/TzI_iKa6wBg264wxORAyviza.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=21360681"),
                },
            };
            return DonamgolCard;
        }

        public static HeroCard GetGrandMamaCard()
        {
            var GrandMamaCard = new HeroCard
            {
                Title = "할매순대국",
                Subtitle = "순대국",
                Text = "순대국 전문점\n\n" +
                "전화번호 : 02-742-2655",
                Images = new List<CardImage> { new CardImage("http://ldb.phinf.naver.net/20190426_257/1556242852138o8NXJ_JPEG/8eYVC2c688j8e9aINbA89C5p.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "메뉴/위치보기", value: "https://store.naver.com/restaurants/detail?id=37373822"),
                },
            };
            return GrandMamaCard;
        }
    }
}