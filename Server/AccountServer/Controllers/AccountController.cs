using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AccountServer.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedDB;
using static AccountServer.DB.DataModel;

namespace AccountServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        AppDbContext _context;
        SharedDbContext _shared;

        public AccountController(AppDbContext context, SharedDbContext shared)
        {
            _context = context;
            _shared = shared;
        }

        [HttpPost]
        [Route("create")]
        public CreateAccountPacketRes CreateAccount([FromBody] CreateAccountPacketReq req)
        {
            CreateAccountPacketRes res = new CreateAccountPacketRes();

            string pattern = @"^\s*$";
            if (Regex.IsMatch(req.AccountName, pattern) || Regex.IsMatch(req.Password, pattern))
            {
                // 문자열이 비어 있거나 공백만 포함되어 있음
                res.CreateOk = false;
                return res;
            }

            // 계정 이름이 이미 존재하는지 확인
            bool isExistingAccount = _context.Accounts.Any(a => a.AccountName == req.AccountName);
            if (isExistingAccount)
            {
                res.CreateOk = false; // 이미 존재하는 계정 이름
                return res;
            }

            // 새로운 계정 생성
            AccountDb newAccount = new AccountDb()
            {
                AccountName = req.AccountName,
                Password = req.Password
            };
            _context.Accounts.Add(newAccount);
            bool success = _context.SaveChangesEx();
            res.CreateOk = success;

            return res;
        }

        [HttpPost]
        [Route("login")]
        public LoginAccountPacketRes LoginAccount([FromBody] LoginAccountPacketReq req)
        {
            LoginAccountPacketRes res = new LoginAccountPacketRes();

            // 입력된 계정 이름과 비밀번호가 공백이거나 비어 있는지 확인
            string pattern = @"^\s*$";
            if (Regex.IsMatch(req.AccountName, pattern) || Regex.IsMatch(req.Password, pattern))
            {
                res.LoginOk = false;
                return res;
            }

            // 계정 인증
            AccountDb account = _context.Accounts.FirstOrDefault(a => a.AccountName == req.AccountName && a.Password == req.Password);
            if (account == null)
            {
                res.LoginOk = false; // 계정 인증 실패
                return res;
            }

            res.LoginOk = true;

            // 토큰 관리
            TokenDb tokenDb = _shared.Tokens.FirstOrDefault(t => t.AccountDbId == account.AccountDbId);
            DateTime expired = DateTime.UtcNow.AddSeconds(600);
            if (tokenDb != null)
            {
                tokenDb.Token = new Random().Next(Int32.MinValue, Int32.MaxValue);
                tokenDb.Expired = expired;
            }
            else
            {
                tokenDb = new TokenDb()
                {
                    AccountDbId = account.AccountDbId,
                    Token = new Random().Next(Int32.MinValue, Int32.MaxValue),
                    Expired = expired
                };
                _shared.Add(tokenDb);
            }
            _shared.SaveChangesEx();

            res.AccountId = account.AccountDbId;
            res.Token = tokenDb.Token;

            // 서버 목록 조회
            res.ServerList = _shared.Servers.Select(serverDb => new ServerInfo()
            {
                Name = serverDb.Name,
                IpAddress = serverDb.IpAddress,
                Port = serverDb.Port,
                BusyScore = serverDb.BusyScore,
            }).ToList();

            return res;
        }
    }
}
