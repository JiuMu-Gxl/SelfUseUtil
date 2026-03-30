using NewLife.Data;
using NewLife.Map.Models;
using NewLife.Map;
using NewLife;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NewLife.Serialization;

namespace SelfUseUtil
{
    //
    // 摘要:
    //     百度地图
    //
    // 言论：
    //     参考手册 https://lbsyun.baidu.com/index.php?title=webapi
    [DisplayName("百度地图")]
    public class BaiduMap : Map, IMap
    {
        private static readonly List<string> _coordTypes = new List<string> { "", "wgs84ll", "sougou", "gcj02ll", "gcj02mc", "bd09ll", "bd09mc", "amap", "gps" };

        //
        // 摘要:
        //     高德地图
        public BaiduMap()
        {
            base.Server = "https://api.map.baidu.com";
            base.KeyName = "ak";
        }

        //
        // 摘要:
        //     远程调用
        //
        // 参数:
        //   url:
        //     目标Url
        //
        //   result:
        //     结果字段
        protected override async Task<T> InvokeAsync<T>(string url, string? result)
        {
            IDictionary<string, object> dictionary = await base.InvokeAsync<IDictionary<string, object>>(url, result);
            if (dictionary == null || dictionary.Count == 0)
            {
                return null;
            }

            int num = dictionary["status"].ToInt();
            if (num != 0)
            {
                string text = (dictionary["msg"] ?? dictionary["message"])?.ToString() ?? "";
                if (num >= 200 || IsValidKey(text))
                {
                    RemoveKey(base.LastKey, DateTime.Now.AddHours(1.0));
                }

                if (base.ThrowException)
                {
                    throw new Exception(text);
                }

                return null;
            }

            if (result.IsNullOrEmpty())
            {
                return (T)dictionary;
            }

            return (T)dictionary[result];
        }

        //
        // 摘要:
        //     查询地址的经纬度坐标
        //
        // 参数:
        //   address:
        //
        //   city:
        //
        //   coordtype:
        //
        // 言论：
        //     参考手册 https://lbsyun.baidu.com/index.php?title=webapi/guide/webservice-geocoding
        protected async Task<IDictionary<string, object>> GetGeocoderAsync(string address, string? city = null, string? coordtype = null)
        {
            if (address.IsNullOrEmpty())
            {
                throw new ArgumentNullException("address");
            }

            address = HttpUtility.UrlEncode(address);
            city = HttpUtility.UrlEncode(city);
            string url = "/geocoding/v3/?address=" + address + "&city=" + city + "&ret_coordtype=" + coordtype + "&extension_analys_level=1&output=json";
            return await InvokeAsync<IDictionary<string, object>>(url, "result");
        }

        //
        // 摘要:
        //     查询地址获取坐标
        //
        // 参数:
        //   address:
        //     地址
        //
        //   city:
        //     城市
        //
        //   coordtype:
        //
        //   formatAddress:
        //     是否格式化地址
        public async Task<GeoAddress?> GetGeoAsync(string address, string? city = null, string? coordtype = null, bool formatAddress = false)
        {
            IDictionary<string, object> rs = await GetGeocoderAsync(address, city, coordtype);
            if (rs == null || rs.Count == 0)
            {
                return null;
            }

            if (!(rs["location"] is IDictionary<string, object> dictionary) || dictionary.Count < 2)
            {
                return null;
            }

            GeoAddress geo = new GeoAddress
            {
                Location = new GeoPoint(dictionary["lng"], dictionary["lat"])
            };
            if (formatAddress)
            {
                GeoAddress geoAddress = await GetReverseGeoAsync(geo.Location, coordtype);
                if (geoAddress != null)
                {
                    geo = geoAddress;
                }
            }

            geo.Precise = rs["precise"].ToBoolean();
            geo.Confidence = rs["confidence"].ToInt();
            geo.Comprehension = rs["comprehension"].ToInt();
            geo.Level = rs["level"]?.ToString() ?? "";
            return geo;
        }

        //
        // 摘要:
        //     根据坐标获取地址
        //
        // 参数:
        //   point:
        //
        //   coordtype:
        //     坐标系
        //
        // 言论：
        //     参考手册 https://lbsyun.baidu.com/index.php?title=webapi/guide/webservice-geocoding-abroad
        protected async Task<IDictionary<string, object>> GetReverseGeocoderAsync(GeoPoint point, string? coordtype)
        {
            if (point == null || point.Longitude == 0.0 || point.Latitude == 0.0)
            {
                throw new ArgumentNullException("point");
            }

            string url = $"/reverse_geocoding/v3/?location={point.Latitude},{point.Longitude}&extensions_poi=1&extensions_town=true&coordtype={coordtype}&output=json";
            return await InvokeAsync<IDictionary<string, object>>(url, "result");
        }

        //
        // 摘要:
        //     根据坐标获取地址
        //
        // 参数:
        //   point:
        //
        //   coordtype:
        public async Task<GeoAddress?> GetReverseGeoAsync(GeoPoint point, string? coordtype)
        {
            IDictionary<string, object> dictionary = await GetReverseGeocoderAsync(point, coordtype);
            if (dictionary == null || dictionary.Count == 0)
            {
                return null;
            }

            GeoAddress geoAddress = new GeoAddress
            {
                Address = (dictionary["formatted_address"]?.ToString() ?? ""),
                Confidence = dictionary["confidence"].ToInt()
            };
            if (dictionary["location"] is IDictionary<string, object> dictionary2 && dictionary2.Count >= 2)
            {
                geoAddress.Location = new GeoPoint(dictionary2["lng"], dictionary2["lat"]);
            }

            if (dictionary["addressComponent"] is IDictionary<string, object> dictionary3)
            {
                new JsonReader().ToObject(dictionary3, null, geoAddress);
                geoAddress.Code = dictionary3["adcode"].ToInt();
                geoAddress.Township = dictionary3["town"]?.ToString() ?? "";
                geoAddress.Towncode = dictionary3["town_code"].ToInt();
                geoAddress.StreetNumber = dictionary3["street_number"]?.ToString() ?? "";
            }

            if (dictionary.TryGetValue("sematic_description", out var value) && value is string text && !text.IsNullOrEmpty())
            {
                geoAddress.Title = text;
            }

            return geoAddress;
        }

        //
        // 摘要:
        //     计算距离和驾车时间
        //
        // 参数:
        //   origin:
        //
        //   destination:
        //
        //   coordtype:
        //
        //   type:
        //     路径计算的方式和方法
        //
        // 言论：
        //     https://lbsyun.baidu.com/index.php?title=webapi/route-matrix-api-v2
        public async Task<Driving?> GetDistanceAsync(GeoPoint origin, GeoPoint destination, string? coordtype, int type = 13)
        {
            if (origin == null || (origin.Longitude < 1.0 && origin.Latitude < 1.0))
            {
                throw new ArgumentNullException("origin");
            }

            if (destination == null || (destination.Longitude < 1.0 && destination.Latitude < 1.0))
            {
                throw new ArgumentNullException("destination");
            }

            if (type <= 0)
            {
                type = 13;
            }

            string text = coordtype;
            if (!text.IsNullOrEmpty() && text.Length > 6)
            {
                text = text.TrimEnd("ll");
            }

            string url = $"/routematrix/v2/driving?origins={origin.Latitude},{origin.Longitude}&destinations={destination.Latitude},{destination.Longitude}&tactics={type}&coord_type={text}&output=json";
            IList<object> list = await InvokeAsync<IList<object>>(url, "result");
            if (list == null || list.Count == 0)
            {
                return null;
            }

            if (!(list.FirstOrDefault() is IDictionary<string, object> dictionary))
            {
                return null;
            }

            IDictionary<string, object> dictionary2 = dictionary["distance"] as IDictionary<string, object>;
            IDictionary<string, object> dictionary3 = dictionary["duration"] as IDictionary<string, object>;
            return new Driving
            {
                Distance = (dictionary2?["value"].ToInt() ?? 0),
                Duration = (dictionary3?["value"].ToInt() ?? 0)
            };
        }
        
        //
        // 摘要:
        //     计算距离和驾车时间
        //
        // 参数:
        //   origin:
        //
        //   destination:
        //
        //   coordtype:
        //
        //   type:
        //     路径计算的方式和方法
        //
        // 言论：
        //     https://lbsyun.baidu.com/index.php?title=webapi/route-matrix-api-v2
        public async Task<Driving?> GetDistanceExAsync(GeoPoint origin, GeoPoint destination, string stratCity, string endCity, string? coordtype, int type = 13)
        {
            if (origin == null || (origin.Longitude < 1.0 && origin.Latitude < 1.0))
            {
                throw new ArgumentNullException("origin");
            }

            if (destination == null || (destination.Longitude < 1.0 && destination.Latitude < 1.0))
            {
                throw new ArgumentNullException("destination");
            }

            if (type <= 0)
            {
                type = 13;
            }

            string text = coordtype;
            if (!text.IsNullOrEmpty() && text.Length > 6)
            {
                text = text.TrimEnd("ll");
            }
            string url = $"/direction/v2/driving?origin={origin}&destination={destination}&ak={base.AppKey}&origin_region={stratCity}&destination_region={endCity}&coord_type={text}&output=json";
            IList<object> list = await InvokeAsync<IList<object>>(url, "result");
            if (list == null || list.Count == 0)
            {
                return null;
            }

            if (!(list.FirstOrDefault() is IDictionary<string, object> dictionary))
            {
                return null;
            }

            IDictionary<string, object> dictionary2 = dictionary["distance"] as IDictionary<string, object>;
            IDictionary<string, object> dictionary3 = dictionary["duration"] as IDictionary<string, object>;
            return new Driving
            {
                Distance = (dictionary2?["value"].ToInt() ?? 0),
                Duration = (dictionary3?["value"].ToInt() ?? 0)
            };
        }

        //
        // 摘要:
        //     行政区划区域检索
        //
        // 参数:
        //   query:
        //
        //   tag:
        //
        //   region:
        //
        //   coordtype:
        //
        //   formatAddress:
        //
        // 言论：
        //     https://lbsyun.baidu.com/index.php?title=webapi/guide/webservice-placeapi
        public async Task<GeoAddress?> PlaceSearchAsync(string query, string tag, string region, string? coordtype = null, bool formatAddress = true)
        {
            query = HttpUtility.UrlEncode(query);
            tag = HttpUtility.UrlEncode(tag);
            region = HttpUtility.UrlEncode(region);
            string url = "/place/v2/search?output=json&query=" + query + "&tag=" + tag + "&region=" + region + "&city_limit=true&ret_coordtype=" + coordtype;
            IList<object> list = await InvokeAsync<IList<object>>(url, "results");
            if (list == null || list.Count == 0)
            {
                return null;
            }

            object obj = list.FirstOrDefault();
            IDictionary<string, object> rs = obj as IDictionary<string, object>;
            if (rs == null)
            {
                return null;
            }

            GeoAddress geo = new GeoAddress();
            if (rs["location"] is IDictionary<string, object> dictionary && dictionary.Count >= 2)
            {
                geo.Location = new GeoPoint(dictionary["lng"], dictionary["lat"]);
                if (formatAddress && geo.Location != null)
                {
                    GeoAddress geoAddress = await GetReverseGeoAsync(geo.Location, coordtype);
                    if (geoAddress != null)
                    {
                        geo = geoAddress;
                    }
                }

                geo.Name = rs["name"]?.ToString() ?? "";
                string text = rs["address"]?.ToString() ?? "";
                if (!text.IsNullOrEmpty())
                {
                    geo.Address = text;
                }

                return geo;
            }

            return null;
        }

        //
        // 摘要:
        //     IP定位
        //
        // 参数:
        //   ip:
        //
        //   coordtype:
        //
        // 言论：
        //     https://lbsyun.baidu.com/index.php?title=webapi/ip-api
        public async Task<IDictionary<string, object?>?> IpLocationAsync(string ip, string coordtype)
        {
            string url = "/location/ip?ip=" + ip + "&coor=" + coordtype;
            IDictionary<string, object> dictionary = await InvokeAsync<IDictionary<string, object>>(url, null);
            if (dictionary == null || dictionary.Count == 0)
            {
                return null;
            }

            if (!(dictionary["content"] is IDictionary<string, object> dictionary2))
            {
                return null;
            }

            if (dictionary.TryGetValue("address", out var value))
            {
                dictionary2["full_address"] = value;
            }

            if (dictionary2.TryGetValue("address_detail", out var value2))
            {
                if (value2 != null)
                {
                    dictionary2.Merge(value2);
                }

                dictionary2.Remove("address_detail");
            }

            if (dictionary2.TryGetValue("point", out var value3))
            {
                if (value3 != null)
                {
                    dictionary2.Merge(value3);
                }

                dictionary2.Remove("point");
            }

            return dictionary2;
        }

        //
        // 摘要:
        //     坐标转换
        //
        // 参数:
        //   points:
        //     需转换的源坐标
        //
        //   from:
        //     源坐标类型。wgs84ll/gcj02/bd09ll
        //
        //   to:
        //     目标坐标类型。gcj02/bd09ll
        //
        // 言论：
        //     https://lbsyun.baidu.com/index.php?title=webapi/guide/changeposition
        public override async Task<IList<GeoPoint>> ConvertAsync(IList<GeoPoint> points, string from, string to)
        {
            if (points == null || points.Count == 0)
            {
                throw new ArgumentNullException("points");
            }

            if (from.IsNullOrEmpty())
            {
                throw new ArgumentNullException("from");
            }

            if (to.IsNullOrEmpty())
            {
                throw new ArgumentNullException("to");
            }

            //if (!from.EndsWithIgnoreCase("ll", "mc"))
            //{
            //    from += "ll";
            //}

            //if (!to.EndsWithIgnoreCase("ll", "mc"))
            //{
            //    to += "ll";
            //}

            if (from.EqualIgnoreCase(to))
            {
                return points;
            }

            int num = 0;
            int num2 = 0;
            for (int i = 0; i < _coordTypes.Count; i++)
            {
                if (_coordTypes[i].EqualIgnoreCase(from))
                {
                    num = i;
                }

                if (_coordTypes[i].EqualIgnoreCase(to))
                {
                    num2 = i;
                }
            }

            if (num == 0)
            {
                throw new ArgumentOutOfRangeException("from");
            }

            if (num2 == 0)
            {
                throw new ArgumentOutOfRangeException("to");
            }

            string url = string.Format("/geoconv/v2/?coords={0}&from={1}&to={2}&output=json", points.Join(";", (GeoPoint e) => $"{e.Longitude},{e.Latitude}"), num, num2);
            List<GeoPoint> list = new List<GeoPoint>();
            IList<object> list2 = await InvokeAsync<IList<object>>(url, "result");
            if (list2 == null || list2.Count == 0)
            {
                return list;
            }

            foreach (IDictionary<string, object> item in list2.Cast<IDictionary<string, object>>())
            {
                list.Add(new GeoPoint(item["x"], item["y"]));
            }

            return list;
        }
    }
}
