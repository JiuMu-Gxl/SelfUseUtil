using NewLife.Data;
using NewLife.Map.Models;
using NewLife.Map;
using NewLife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NewLife.Serialization;
using SelfUseUtil.Model;

namespace SelfUseUtil
{
    public class AMap : Map, IMap
    {
        private readonly string[] _KeyWords = new string[4] { "TOO_FREQUENT", "LIMIT", "NOMATCH", "RECYCLED" };

        //
        // 摘要:
        //     高德地图
        public AMap()
        {
            base.Server = "http://restapi.amap.com";
            base.KeyName = "key";
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

            if (dictionary["status"].ToInt() != 1)
            {
                string text = dictionary["info"]?.ToString() ?? "";
                if (!base.LastKey.IsNullOrEmpty() && IsValidKey(text))
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
            
            // 👉 把 dictionary[result] 转为 JSON 再反序列化
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary[result]);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
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
        protected async Task<IDictionary<string, object>?> GetGeocoderAsync(string address, string? city = null, string? coordtype = null)
        {
            if (address.IsNullOrEmpty())
            {
                throw new ArgumentNullException("address");
            }

            address = HttpUtility.UrlEncode(address);
            city = HttpUtility.UrlEncode(city);
            string url = "http://restapi.amap.com/v3/geocode/geo?address=" + address + "&city=" + city + "&output=json";
            return (await InvokeAsync<IList<object>>(url, "geocodes"))?.FirstOrDefault() as IDictionary<string, object>;
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
        //     是否格式化地址。高德地图默认已经格式化地址
        public async Task<GeoAddress?> GetGeoAsync(string address, string? city = null, string? coordtype = null, bool formatAddress = false)
        {
            IDictionary<string, object> rs = await GetGeocoderAsync(address, city);
            if (rs == null || rs.Count == 0)
            {
                return null;
            }

            GeoAddress geo = new GeoAddress
            {
                Location = new GeoPoint(rs["location"] as string)
            };
            rs.Remove("location");
            new JsonReader().ToObject(rs, null, geo);
            geo.Code = rs["adcode"].ToInt();
            if (rs["township"] is IList<object> list && list.Count > 0)
            {
                geo.Township = list[0]?.ToString() ?? "";
            }

            if (rs["number"] is IList<object> list2 && list2.Count > 0)
            {
                geo.StreetNumber = list2[0]?.ToString() ?? "";
            }

            if (formatAddress)
            {
                GeoAddress geoAddress = await GetReverseGeoAsync(geo.Location, "gcj02");
                if (geoAddress != null)
                {
                    geo = geoAddress;
                    if (geo.Level.IsNullOrEmpty())
                    {
                        geo.Level = rs["level"]?.ToString() ?? "";
                    }
                }
            }

            string text = rs["formatted_address"]?.ToString() ?? "";
            if (!text.IsNullOrEmpty() && (geo.Address.IsNullOrEmpty() || geo.Address.Length < text.Length))
            {
                geo.Address = text;
            }

            TrimAddress(geo);
            return geo;
        }

        private static void TrimAddress(GeoAddress geo)
        {
            if (!geo.Address.IsNullOrEmpty())
            {
                geo.Address = geo.Address.Replace("|", null);
            }
        }

        //
        // 摘要:
        //     根据坐标获取地址
        //
        // 参数:
        //   point:
        //
        //   coordtype:
        //
        // 言论：
        //     http://lbs.amap.com/api/webservice/guide/api/georegeo/#regeo
        protected async Task<IDictionary<string, object>> GetReverseGeocoderAsync(GeoPoint point, string? coordtype)
        {
            if (point.Longitude < 0.1 || point.Latitude < 0.1)
            {
                throw new ArgumentNullException("point");
            }

            string url = $"http://restapi.amap.com/v3/geocode/regeo?location={point.Longitude},{point.Latitude}&extensions=base&output=json";
            return await InvokeAsync<IDictionary<string, object>>(url, "regeocode");
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
                Location = point
            };
            if (dictionary["addressComponent"] is IDictionary<string, object> dictionary2)
            {
                new JsonReader().ToObject(dictionary2, null, geoAddress);
                if (dictionary2.TryGetValue("city", out var value) && !(value is string))
                {
                    geoAddress.City = null;
                }

                if (dictionary2.TryGetValue("streetNumber", out value) && !(value is string))
                {
                    geoAddress.StreetNumber = null;
                }

                geoAddress.Code = dictionary2["adcode"].ToInt();
                geoAddress.Township = null;
                geoAddress.Towncode = 0;
                string text = "";
                if (dictionary2["township"] is string township)
                {
                    geoAddress.Township = township;
                }

                if (dictionary2["towncode"] is string text2)
                {
                    text = text2;
                }

                if (!text.IsNullOrEmpty() && text.Length > 9)
                {
                    text = text.TrimEnd("000");
                }

                geoAddress.Towncode = text.ToInt();
                if (dictionary2["streetNumber"] is IDictionary<string, object> dictionary3 && dictionary3.Count > 0)
                {
                    if (dictionary3["street"] is IList<object> list && list.Count > 0)
                    {
                        geoAddress.Street = list[0]?.ToString() ?? "";
                    }
                    else
                    {
                        geoAddress.Street = dictionary3["street"]?.ToString() ?? "";
                    }

                    if (dictionary3["number"] is IList<object> list2 && list2.Count > 0)
                    {
                        geoAddress.StreetNumber = list2[0]?.ToString() ?? "";
                    }
                    else
                    {
                        geoAddress.StreetNumber = dictionary3["number"]?.ToString() ?? "";
                    }

                    if (geoAddress.Title.IsNullOrEmpty())
                    {
                        if (!dictionary3.TryGetValue("direction", out var value2))
                        {
                            value2 = "";
                        }

                        if (dictionary3.TryGetValue("distance", out var value3))
                        {
                            value3 = Math.Round(value3.ToDouble(), 0) + "米";
                        }

                        geoAddress.Title = $"{geoAddress.Province}{geoAddress.City}{geoAddress.District}{geoAddress.Township}{geoAddress.Street}{geoAddress.StreetNumber}{value2}{value3}";
                    }
                }
            }

            geoAddress.Location = point;
            TrimAddress(geoAddress);
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
        //     http://lbs.amap.com/api/webservice/guide/api/direction type: 0：直线距离 1：驾车导航距离（仅支持国内坐标）。
        //     必须指出，当为1时会考虑路况，故在不同时间请求返回结果可能不同。 此策略和driving接口的 strategy = 4策略一致 2：公交规划距离（仅支持同城坐标）
        //     3：步行规划距离（仅支持5km之间的距离） distance 路径距离，单位：米 duration 预计行驶时间，单位：秒
        public async Task<Driving?> GetDistanceAsync(GeoPoint origin, GeoPoint destination, string? coordtype, int type = 1)
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
                type = 1;
            }

            string url = $"http://restapi.amap.com/v3/distance?origins={origin.Longitude},{origin.Latitude}&destination={destination.Longitude},{destination.Latitude}&type={type}&output=json";
            IList<object> list = await InvokeAsync<IList<object>>(url, "results");
            if (list == null || list.Count == 0)
            {
                return null;
            }

            if (!(list.FirstOrDefault() is IDictionary<string, object> dictionary))
            {
                return null;
            }

            return new Driving
            {
                Distance = dictionary["distance"].ToInt(),
                Duration = dictionary["duration"].ToInt()
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
        //     http://lbs.amap.com/api/webservice/guide/api/direction type: 0：直线距离 1：驾车导航距离（仅支持国内坐标）。
        //     必须指出，当为1时会考虑路况，故在不同时间请求返回结果可能不同。 此策略和driving接口的 strategy = 4策略一致 2：公交规划距离（仅支持同城坐标）
        //     3：步行规划距离（仅支持5km之间的距离） distance 路径距离，单位：米 duration 预计行驶时间，单位：秒
        public async Task<DrivingEx?> GetDistanceExAsync(GeoPoint origin, GeoPoint destination, string? coordtype, int type = 1)
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
                type = 1;
            }

            string url = $"https://restapi.amap.com/v5/direction/driving?origin={origin.Longitude},{origin.Latitude}&destination={destination.Longitude},{destination.Latitude}&key=c7591c54f7443323b41ec4c0f315acf5&output=json&show_fields=cost,polyline";
            var route = await InvokeAsync<Route>(url, "route");
            var list = route.paths;
            if (route.paths == null || route.paths.Count == 0)
            {
                return null;
            }
            return new DrivingEx
            {
                Distance = route.paths[0].distance.ToInt(),
                Duration = route.paths[0].cost.duration.ToInt(),
                Steps = route.paths[0].steps
            };
        }

        //
        // 摘要:
        //     行政区划
        //
        // 参数:
        //   keywords:
        //     查询关键字
        //
        //   subdistrict:
        //     设置显示下级行政区级数
        //
        //   code:
        //     按照指定行政区划进行过滤，填入后则只返回该省/直辖市信息
        //
        // 言论：
        //     http://lbs.amap.com/api/webservice/guide/api/district
        public async Task<IList<GeoArea>> GetAreaAsync(string keywords, int subdistrict = 1, int code = 0)
        {
            if (keywords.IsNullOrEmpty())
            {
                throw new ArgumentNullException("keywords");
            }

            keywords = HttpUtility.UrlEncode(keywords);
            string url = $"http://restapi.amap.com/v3/config/district?keywords={keywords}&subdistrict={subdistrict}&filter={code}&extensions=base&output=json";
            IList<object> list = await InvokeAsync<IList<object>>(url, "districts");
            if (list == null || list.Count == 0)
            {
                return new List<GeoArea>();
            }

            if (!(list.FirstOrDefault() is IDictionary<string, object> geo))
            {
                return new List<GeoArea>();
            }

            return GetArea(geo, 0);
        }

        private IList<GeoArea> GetArea(IDictionary<string, object> geo, int parentCode)
        {
            if (geo == null || geo.Count == 0)
            {
                return new List<GeoArea>();
            }

            List<GeoArea> list = new List<GeoArea>();
            GeoArea geoArea = new GeoArea();
            new JsonReader().ToObject(geo, null, geoArea);
            geoArea.Code = geo["adcode"].ToInt();
            if (parentCode > 0)
            {
                geoArea.ParentCode = parentCode;
            }

            list.Add(geoArea);
            if (geo["districts"] is IList<object> list2 && list2.Count > 0)
            {
                foreach (object item in list2)
                {
                    if (item is IDictionary<string, object> geo2)
                    {
                        IList<GeoArea> area = GetArea(geo2, geoArea.Code);
                        if (area != null && area.Count > 0)
                        {
                            list.AddRange(area);
                        }
                    }
                }
            }

            return list;
        }

        //
        // 摘要:
        //     是否无效Key。可能禁用或超出限制
        //
        // 参数:
        //   result:
        protected override bool IsValidKey(string result)
        {
            if (result.IsNullOrEmpty())
            {
                return false;
            }

            if (_KeyWords.Any(result.Contains))
            {
                return true;
            }

            return base.IsValidKey(result);
        }
    }
}
