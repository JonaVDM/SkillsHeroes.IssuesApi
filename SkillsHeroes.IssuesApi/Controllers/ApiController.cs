using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillsHeroes.IssuesApi.Data;
using SkillsHeroes.IssuesApi.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SkillsHeroes.IssuesApi.Controllers
{
    [Route("api")]
    [Authorize]
    [ApiController]
    [Produces("application/json", "text/json")]
    public class ApiController : Controller
    {
        private const string ENUM_DEFINED_URGENCY = "Urgency must be a defined urgency";
        private readonly Expression<Func<Issue, Models.Issue>> _convertDbIssueToModelIssue = i => new Models.Issue()
        {
            Id = i.Id,
            Title = i.Title,
            Description = i.Description,
            Urgency = i.Urgency,
            Created = i.Created,
            InProcess = i.InProcess,
            Completed = i.Completed,

            Comments = i.Comments.Select(c => new Models.Comment()
            {
                Id = c.Id,
                Created = c.Created,
                Text = c.Text
            }).ToArray()
        };
        private readonly IssuesContext _context;
        private readonly Application _application;

        public ApiController(IssuesContext context, Application application)
        {
            _context = context;
            _application = application;
        }

        /// <summary>
        /// Get all issues
        /// </summary>
        /// <returns>All issues</returns>
        [HttpGet("issues")]
        public async Task<ActionResult<IEnumerable<Models.Issue>>> Get(
            Models.Filter filter = Models.Filter.All,
            Models.OrderField order_field = Models.OrderField.CreationDate,
            Models.Order order = Models.Order.Ascending)
        {
            if (!Enum.IsDefined(typeof(Models.Filter), filter))
            {
                return BadRequest("filter must be a defined filter");
            }
            if (!Enum.IsDefined(typeof(Models.OrderField), order_field))
            {
                return BadRequest("order_field must be a defined orderfield");
            }
            if (!Enum.IsDefined(typeof(Models.Order), order))
            {
                return BadRequest("order must be a defined order");
            }

            var query = _issuesQuery();

            switch (filter)
            {
                case Models.Filter.Completed:
                    query = query.Where(i => i.Completed != null);
                    break;
                case Models.Filter.In_Process:
                    query = query.Where(i => i.Completed == null && i.InProcess != null);
                    break;
                case Models.Filter.New:
                    query = query.Where(i => i.Completed == null && i.InProcess == null);
                    break;
            }

            var items = await query
                .Include(i => i.Comments)
                .Select(_convertDbIssueToModelIssue)
                .ToArrayAsync();

            if (order == Models.Order.Ascending)
            {
                switch (order_field)
                {
                    case Models.OrderField.CreationDate:
                        return items.OrderBy(i => i.Created).ToArray();
                    case Models.OrderField.Title:
                        return items.OrderBy(i => i.Title).ToArray();
                    case Models.OrderField.Urgency:
                        return items.OrderBy(i => i.Urgency).ToArray();
                    default:
                        throw new InvalidOperationException();
                }
            }
            else
            {
                switch (order_field)
                {
                    case Models.OrderField.CreationDate:
                        return items.OrderByDescending(i => i.Created).ToArray();
                    case Models.OrderField.Title:
                        return items.OrderByDescending(i => i.Title).ToArray();
                    case Models.OrderField.Urgency:
                        return items.OrderByDescending(i => i.Urgency).ToArray();
                    default:
                        throw new InvalidOperationException();
                }
            }

        }

        /// <summary>
        /// Get issue by id
        /// </summary>
        /// <param name="id">The id of the issue you wish to get</param>
        /// <returns>The requested issue</returns>
        [HttpGet("issue/{id}")]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Models.Issue>> Get(int id)
        {
            var issue = await _issuesQuery()
                .Include(i => i.Comments)
                .SingleOrDefaultAsync(i => i.Id == id);

            if (issue == null)
                return NotFound();

            return _convertDbIssueToModelIssue.Compile().Invoke(issue);
        }

        /// <summary>
        /// Add new issue
        /// </summary>
        /// <param name="issue">The issue that you wish to add</param>
        /// <returns>The newly created issue</returns>
        [HttpPost("add")]
        [ProducesResponseType(typeof(Models.Issue), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Models.Issue>> Post([FromBody] Models.AddIssue issue)
        {
            if (string.IsNullOrWhiteSpace(issue.Title))
            {
                return BadRequest("Issue must have a title");
            }
            if (string.IsNullOrWhiteSpace(issue.Description))
            {
                return BadRequest("Issue must have a description");
            }
            if (!Enum.IsDefined(typeof(Urgency), issue.Urgency))
            {
                return BadRequest(ENUM_DEFINED_URGENCY);
            }

            var dbIssue = new Issue()
            {
                ApplicationId = _application.Id,
                Title = issue.Title,
                Description = issue.Description,
                Urgency = issue.Urgency,
                Comments = new HashSet<Comment>()
            };

            _context.Add(dbIssue);

            await _context.SaveChangesAsync();

            return Created(Url.Action("Get", "Api", new { id = dbIssue.Id }), _convertDbIssueToModelIssue.Compile().Invoke(dbIssue));
        }

        /// <summary>
        /// Change the issue urgency
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("edit/{id}")]
        [ProducesResponseType(typeof(Models.Issue), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Models.Issue>> Put(int id, [FromBody] Models.UrgencyUpdate value)
        {
            if (!Enum.IsDefined(typeof(Urgency), value.Urgency))
            {
                return BadRequest(ENUM_DEFINED_URGENCY);
            }

            var issue = await _issuesQuery()
                .Include(i => i.Comments)
                .SingleOrDefaultAsync(i => i.Id == id);

            if (issue == null)
            {
                return NotFound();
            }

            if (issue.Completed != null)
            {
                return BadRequest("Issue is already completed");
            }

            issue.Urgency = value.Urgency;
            await _context.SaveChangesAsync();

            return _convertDbIssueToModelIssue.Compile().Invoke(issue);
        }

        /// <summary>
        /// Change issue to in process
        /// </summary>
        /// <param name="id">The id of the issue you wish to change</param>
        /// <returns>A 200 OK result</returns>
        [HttpPost("in_process/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InProcess(int id)
        {
            var issue = await _issuesQuery().SingleOrDefaultAsync(i => i.Id == id);

            if (issue == null)
            {
                return NotFound();
            }

            if (issue.InProcess != null)
            {
                return BadRequest("Issue is already in process");
            }

            issue.InProcess = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Change issue to completed
        /// </summary>
        /// <param name="id">The id of the issue you wish to change</param>
        /// <returns>A 200 OK result</returns>
        [HttpPost("complete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Complete(int id)
        {
            var issue = await _issuesQuery().SingleOrDefaultAsync(i => i.Id == id);

            if (issue == null)
            {
                return NotFound();
            }

            if (issue.Completed != null)
            {
                return BadRequest("Issue is already completed");
            }

            issue.Completed = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Add a comment to an issue
        /// </summary>
        /// <param name="id">The id of the issue you wish to add a comment to</param>
        /// <param name="value">The new comment</param>
        /// <returns>A 200 OK result</returns>
        [HttpPost("add_comment/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddComment(int id, [FromBody] Models.AddComment value)
        {
            if (string.IsNullOrWhiteSpace(value.Text))
            {
                return BadRequest("Comment can not be null or empty");
            }

            var issue = await _issuesQuery().SingleOrDefaultAsync(i => i.Id == id);

            if (issue == null)
            {
                return NotFound();
            }

            var comment = new Comment()
            {
                Text = value.Text,
                IssueId = issue.Id,
                Issue = issue
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private IQueryable<Issue> _issuesQuery() => _context.Issues.Where(i => i.ApplicationId == _application.Id);
    }
}
